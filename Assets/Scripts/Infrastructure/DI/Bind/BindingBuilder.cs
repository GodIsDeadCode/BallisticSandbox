using System;
using BallisticSandbox.Infrastructure.DI.Container;
using BallisticSandbox.Infrastructure.DI.Utility;

namespace BallisticSandbox.Infrastructure.DI.Bind
{
    public class BindingBuilder : IBindingConfigurationState, IBindingImplementationState, IBindingLifetimeState, IBindingContractState
    {
        private readonly IDependencyRegistry _dependencyRegistry;
        private readonly BindingBuilderContext _context;

        public BindingBuilder(IDependencyRegistry dependencyRegistry)
        {
            _dependencyRegistry = dependencyRegistry;
            _context = new BindingBuilderContext();
        }

        public IBindingConfigurationState AsScoped()
        {
            _context.Lifetime = Lifetime.Scoped;
            return this;
        }

        public IBindingConfigurationState AsSingleton()
        {
            _context.Lifetime = Lifetime.Singleton;
            return this;
        }

        public IBindingConfigurationState AsTransient()
        {
            _context.Lifetime = Lifetime.Transient;
            return this;
        }

        public IBindingImplementationState Bind<TContract>() where TContract : class
        {
            return Bind(typeof(TContract));
        }

        public IBindingImplementationState Bind(Type contractType)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType), "Contract type cannot be null.");

            if (contractType.IsValueType)
                throw new ArgumentException("Contract type cannot be a value type.", nameof(contractType));

            _context.ContractType = contractType;
            return this;
        }

        public void CommitBinding()
        {
            if (_context.ContractType == null)
                throw new InvalidOperationException("Contract type must be specified before committing the binding.");

            if (_context.ImplementationType == null && _context.Factory == null && _context.Instance == null)
                throw new InvalidOperationException("At least one of the following must be specified before committing the binding: implementation type, factory, or instance.");

            _dependencyRegistry.Register(_context.BuildBindData());
            _context.Reset();
        }

        public IBindingContractState SetupBinding()
        {
            return this;
        }

        public IBindingLifetimeState To<TImplementation>() where TImplementation : class
        {
            return To(typeof(TImplementation));
        }

        public IBindingLifetimeState To(Type implementationType)
        {
            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType), "Implementation type cannot be null.");

            ValidateImplType(implementationType);

            _context.ImplementationType = implementationType;
            return this;
        }

        public IBindingLifetimeState ToFactory(Func<IDependencyResolver, object> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory), "Factory cannot be null.");

            Type type = factory.Method.ReturnType;
            ValidateImplType(type);

            _context.ImplementationType = type;
            _context.Factory = factory;
            return this;
        }

        public IBindingLifetimeState ToInstance(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance), "Instance cannot be null.");

            Type type = instance.GetType();
            ValidateImplType(type);

            _context.ImplementationType = type;
            _context.Instance = instance;
            return this;
        }

        public IBindingLifetimeState ToSelf()
        {
            ValidateImplType(_context.ContractType);

            _context.ImplementationType = _context.ContractType;
            return this;
        }

        public IBindingConfigurationState WithArgument(int position, Type type, object value)
        {
            if (position < 0)
                throw new ArgumentOutOfRangeException(nameof(position), "Position must be a non-negative integer.");

            if (type == null)
                throw new ArgumentNullException(nameof(type), "Type cannot be null.");

            _context.Arguments.Add(position, new TypeValuePair(type, value));
            return this;
        }

        public IBindingConfigurationState WithArguments(params TypeValuePair[] arguments)
        {
           if (arguments == null)
                throw new ArgumentNullException(nameof(arguments), "Arguments cannot be null.");

            for (int i = 0; i < arguments.Length; i++)
            {
                TypeValuePair arg = arguments[i];
                if (arg.Type == null)
                    throw new ArgumentException($"Type of argument at position {i} cannot be null.", nameof(arguments));

                _context.Arguments.Add(i, arg);
            }

            return this;
        }

        public IBindingConfigurationState WithIdentifier(object identifier)
        {
            _context.Identifier = identifier;
            return this;
        }

        private void ValidateImplType(Type impType)
        {
            if (impType.IsValueType)
                throw new ArgumentException("Implementation type cannot be a value type.", nameof(impType));

            if (impType.IsAbstract || impType.IsInterface)
                throw new ArgumentException("Implementation type cannot be abstract or an interface.", nameof(impType));

            if (!_context.ContractType.IsAssignableFrom(impType))
                throw new ArgumentException("Implementation type must be assignable to the contract type.", nameof(impType));
        }
    }
}
