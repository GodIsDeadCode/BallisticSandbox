using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using BallisticSandbox.Infrastructure.DI.Attribute;
using BallisticSandbox.Infrastructure.DI.Container;
using BallisticSandbox.Infrastructure.DI.Utility;

namespace BallisticSandbox.Infrastructure.DI.Factory
{
    public class InstanceFactory : IInstanceFactory
    {
        private static readonly MethodInfo ResolveMethod;
        private static readonly ParameterExpression ResolverParameter;
        private static readonly PropertyInfo DictionaryItemProperty;
        private static readonly MethodInfo ContainsKeyMethod;
        private static readonly Expression NullObjectConstant;
        private static readonly ParameterExpression ArgumentsDictParameter;
        private static readonly ConstructorInfo ArgumentExceptionCtor;

        private readonly IDependencyResolver _dependencyResolver;
        private readonly ConcurrentDictionary<Type, Func<IDependencyResolver, IReadOnlyDictionary<int, TypeValuePair>, object>> _factoriesCache;

        public InstanceFactory(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
            _factoriesCache = new ConcurrentDictionary<Type, Func<IDependencyResolver, IReadOnlyDictionary<int, TypeValuePair>, object>>();
        }

        static InstanceFactory()
        {
            ResolveMethod = typeof(IDependencyResolver).GetMethod(nameof(IDependencyResolver.Resolve), new[] { typeof(Type), typeof(object) });
            ResolverParameter = Expression.Parameter(typeof(IDependencyResolver), "resolver");
            DictionaryItemProperty = typeof(IReadOnlyDictionary<Type, Func<IDependencyResolver, object>>).GetProperty("Item");
            ContainsKeyMethod = typeof(IReadOnlyDictionary<Type, Func<IDependencyResolver, object>>).GetMethod(nameof(IDictionary<Type, Func<IDependencyResolver, object>>.ContainsKey));
            NullObjectConstant = Expression.Constant(null, typeof(object));
            ArgumentExceptionCtor = typeof(ArgumentException).GetConstructor(new[] { typeof(string) });
            ArgumentsDictParameter = Expression.Parameter(typeof(IReadOnlyDictionary<int, TypeValuePair>), "arguments");
        }

        public T CreateInstance<T>(IReadOnlyDictionary<int, TypeValuePair> arguments = null)
        {
            return (T)CreateInstance(typeof(T), arguments);
        }

        public object CreateInstance(Type type, IReadOnlyDictionary<int, TypeValuePair> arguments = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type), "Type cannot be null.");

            return GetOrCreateDelegate(type).Invoke(_dependencyResolver, arguments);
        }

        private Func<IDependencyResolver, IReadOnlyDictionary<int, TypeValuePair>, object> GetOrCreateDelegate(Type type)
        {
            if (_factoriesCache.TryGetValue(type, out Func<IDependencyResolver, IReadOnlyDictionary<int, TypeValuePair>, object> factory))
                return factory;

            factory = CreateDelegateInternal(type);
            _factoriesCache[type] = factory;

            return factory;
        }

        private Func<IDependencyResolver, IReadOnlyDictionary<int, TypeValuePair>, object> CreateDelegateInternal(Type type, IReadOnlyDictionary<int, TypeValuePair> arguments = null)
        {
            ConstructorData constructorData = GetConstructorData(type);
            NewExpression newExpression = null;

            if (constructorData.Parameters.Length == 0)
                newExpression = Expression.New(constructorData.Constructor);
            else
                newExpression = Expression.New(constructorData.Constructor,
                                               GetConstructorParamExpressions(constructorData, ArgumentsDictParameter));

            return Expression.Lambda<Func<IDependencyResolver, IReadOnlyDictionary<int, TypeValuePair>, object>>(newExpression, ResolverParameter, ArgumentsDictParameter).Compile();
        }

        private Expression[] GetConstructorParamExpressions(ConstructorData constructorData, ParameterExpression argumentsExpression)
        {
            ParameterInfo[] parameters = constructorData.Parameters;
            Expression[] paramExpressions = new Expression[parameters.Length];

            Expression nullDictExpression = Expression.Constant(null, typeof(IReadOnlyDictionary<Type, TypeValuePair>));
            BinaryExpression argumentsNotNull = Expression.Equal(argumentsExpression, nullDictExpression);

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                Type parameterType = parameter.ParameterType;

                Expression indexExpression = Expression.Constant(i, typeof(int));
                MethodCallExpression containsKeyCallExp = Expression.Call(argumentsExpression, ContainsKeyMethod, indexExpression);
                BinaryExpression hasArgument = Expression.AndAlso(argumentsNotNull, containsKeyCallExp);

                Expression valueExpression = BuildValueExpression(argumentsExpression,
                                                                  indexExpression,
                                                                  parameter,
                                                                  constructorData,
                                                                  i);

                Expression fallbackExpression = BuildFallbackExpression(parameter,
                                                                        constructorData,
                                                                        i);

                paramExpressions[i] = Expression.Condition(hasArgument, valueExpression, fallbackExpression);
            }

            return paramExpressions;
        }

        private Expression BuildValueExpression(ParameterExpression argumentExp, Expression indexExp, ParameterInfo parameter, ConstructorData constructorData, int index)
        {
            IndexExpression pairExpression = Expression.Property(argumentExp, DictionaryItemProperty, indexExp);
            MemberExpression vaueExpression = Expression.Property(pairExpression, nameof(TypeValuePair.Value));
            UnaryExpression castExpression = Expression.Convert(vaueExpression, parameter.ParameterType);

            bool isNullable = !parameter.ParameterType.IsValueType || Nullable.GetUnderlyingType(parameter.ParameterType) != null;
            if (isNullable)
                return castExpression;

            return BuildNullGuardedExpression(castExpression, parameter, constructorData, index);
        }

        private Expression BuildNullGuardedExpression(UnaryExpression valueExpression, ParameterInfo parameter, ConstructorData constructorData, int index)
        {
            BinaryExpression isNull = Expression.Equal(valueExpression, NullObjectConstant);
            ConstantExpression messsageExpression = Expression.Constant($"Argument '{parameter.Name}' " +
                                                                        $"of type '{parameter.ParameterType.FullName}' cannot be null for constructor " +
                                                                        $"of type '{constructorData.Constructor.DeclaringType.FullName}'.", typeof(string));

            UnaryExpression throwExpression = Expression.Throw(Expression.New(ArgumentExceptionCtor, messsageExpression), parameter.ParameterType);
            return Expression.Condition(isNull, throwExpression, valueExpression);
        }

        private Expression BuildFallbackExpression(ParameterInfo parameter, ConstructorData constructorData, int index)
        {
            if (parameter.HasDefaultValue)
                return Expression.Constant(parameter.DefaultValue, parameter.ParameterType);

            MethodCallExpression resolveCallExpression = Expression.Call(ResolverParameter,
                                                                         ResolveMethod,
                                                                         Expression.Constant(parameter.ParameterType, typeof(Type)),
                                                                         Expression.Constant(constructorData.Identifiers[index], typeof(object)));

            return Expression.Convert(resolveCallExpression, parameter.ParameterType);
        }

        private ConstructorData GetConstructorData(Type type)
        {
            ConstructorInfo constructor = FindConstructor(type);
            ParameterInfo[] parameters = constructor.GetParameters();
            object[] identifiers = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                identifiers[i] = parameters[i].GetCustomAttribute<IdentifierAttribute>()?.Identifier;

            return new ConstructorData(constructor, parameters, identifiers);
        }

        private ConstructorInfo FindConstructor(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            if (constructors.Length == 0)
                throw new InvalidOperationException($"Type {type.FullName} does not have a public constructor.");

            ConstructorInfo selectedConstructor = null;
            int maxParameters = -1;

            for (int i = 0; i < constructors.Length; i++)
            {
                ConstructorInfo constructor = constructors[i];
                if (constructor.IsDefined(typeof(InjectAttribute), false))
                {
                    if (selectedConstructor != null)
                        throw new InvalidOperationException($"Multiple constructors of type {type.FullName} are marked with [Inject].");

                    selectedConstructor = constructor;
                }
                else if (selectedConstructor == null)
                {
                    int parameterCount = constructor.GetParameters().Length;
                    if (parameterCount > maxParameters)
                    {
                        maxParameters = parameterCount;
                        selectedConstructor = constructor;
                    }
                }
            }

            return selectedConstructor;
        }
    }
}
