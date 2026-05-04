using System;
using System.Collections.Generic;
using System.Reflection;
using BallisticSandbox.Infrastructure.DI.Attribute;
using BallisticSandbox.Infrastructure.DI.Container;
using BallisticSandbox.Infrastructure.DI.Exceptions;

namespace BallisticSandbox.Infrastructure.DI.Injection
{
    public class DependencyInjector : IDependencyInjector
    {
        private const BindingFlags MemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        private readonly IDependencyResolver _dependencyResolver;
        private readonly Dictionary<Type, List<InjectableField>> _injectableFields;
        private readonly Dictionary<Type, List<InjectableProperty>> _injectableProperties;
        private readonly Dictionary<Type, List<InjectableMethod>> _injectableMethods;

        public DependencyInjector(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;

            _injectableFields = new Dictionary<Type, List<InjectableField>>();
            _injectableProperties = new Dictionary<Type, List<InjectableProperty>>();
            _injectableMethods = new Dictionary<Type, List<InjectableMethod>>();
        }

        public void Inject(object instance, InjectionType injectionType)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance), "Instance cannot be null.");

            InjectInternal(instance.GetType(), instance, injectionType);
        }

        private void InjectInternal(Type type, object instance, InjectionType injectionType)
        {
            try
            {
                if (injectionType.HasFlag(InjectionType.Properties))
                    InjectIntoProperties(type, instance);

                if (injectionType.HasFlag(InjectionType.Fields))
                    InjectIntoFields(type, instance);

                if (injectionType.HasFlag(InjectionType.Methods))
                    InjectIntoMethods(type, instance);
            }
            catch (DependencyResolutionException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InjectionException($"[Inject] Failed to inject into {type.FullName}.", type, ex);
            }
        }

        private void InjectIntoFields(Type type, object instance)
        {
            List<InjectableField> injectableFields = GetOrCacheInjectableFields(type);
            for (int i = 0; i < injectableFields.Count; i++)
            {
                InjectableField injectableField = injectableFields[i];
                object value = _dependencyResolver.Resolve(injectableField.Field.FieldType, injectableField.Identifier);
                injectableField.Field.SetValue(instance, value);
            }
        }

        private void InjectIntoProperties(Type type, object instance)
        {
            List<InjectableProperty> injectableProperties = GetOrCacheInjectableProperties(type);
            for (int i = 0; i < injectableProperties.Count; i++)
            {
                InjectableProperty injectableProperty = injectableProperties[i];
                object value = _dependencyResolver.Resolve(injectableProperty.Property.PropertyType, injectableProperty.Identifier);
                injectableProperty.Property.SetValue(instance, value);
            }
        }

        private void InjectIntoMethods(Type type, object instance)
        {
            List<InjectableMethod> injectableMethods = GetOrCacheInjectableMethods(type);
            for (int i = 0; i < injectableMethods.Count; i++)
            {
                InjectableMethod injectableMethod = injectableMethods[i];
                object[] args = new object[injectableMethod.Parameters.Length];

                for (int j = 0; j < args.Length; j++)
                    args[j] = _dependencyResolver.Resolve(injectableMethod.Parameters[j].ParameterType, injectableMethod.Identifiers[i]);

                injectableMethod.Method.Invoke(instance, args);
            }
        }

        private List<InjectableField> GetOrCacheInjectableFields(Type type)
        {
            if (_injectableFields.TryGetValue(type, out List<InjectableField> result))
                return result;

            result = new List<InjectableField>();
            FieldInfo[] fields = type.GetFields(MemberFlags);

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                if (!field.IsDefined(typeof(InjectAttribute), false))
                    continue;

                if (field.IsInitOnly)
                    throw new InvalidOperationException($"Field {field.Name} in {type.Name} is marked [Inject] but is readonly.");

                object identifier = field.GetCustomAttribute<IdentifierAttribute>()?.Identifier;
                result.Add(new InjectableField(field, identifier));
            }

            _injectableFields[type] = result;
            return result;
        }

        private List<InjectableProperty> GetOrCacheInjectableProperties(Type type)
        {
            if (_injectableProperties.TryGetValue(type, out List<InjectableProperty> result))
                return result;

            result = new List<InjectableProperty>();
            PropertyInfo[] properties = type.GetProperties(MemberFlags);

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                if (!property.IsDefined(typeof(InjectAttribute), false))
                    continue;

                if (!property.CanWrite)
                    throw new InvalidOperationException($"Property {property.Name} in {type.Name} is marked [Inject] but has no setter.");

                object identifier = property.GetCustomAttribute<IdentifierAttribute>()?.Identifier;
                result.Add(new InjectableProperty(property, identifier));
            }

            _injectableProperties[type] = result;
            return result;
        }

        private List<InjectableMethod> GetOrCacheInjectableMethods(Type type)
        {
            if (_injectableMethods.TryGetValue(type, out List<InjectableMethod> result))
                return result;

            result = new List<InjectableMethod>();
            MethodInfo[] methods = type.GetMethods(MemberFlags);

            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];
                if (!method.IsDefined(typeof(InjectAttribute), false))
                    continue;

                ParameterInfo[] parameters = method.GetParameters();
                object[] identifiers = new object[parameters.Length];

                for (int j = 0; j < parameters.Length; j++)
                    identifiers[j] = parameters[j].GetCustomAttribute<IdentifierAttribute>()?.Identifier;

                result.Add(new InjectableMethod(method, parameters, identifiers));
            }

            _injectableMethods[type] = result;
            return result;
        }
    }

    [Flags]
    public enum InjectionType
    {
        None = 0,
        Fields = 1,
        Properties = 2,
        Methods = 4,
        All = Fields | Properties | Methods
    }
}
