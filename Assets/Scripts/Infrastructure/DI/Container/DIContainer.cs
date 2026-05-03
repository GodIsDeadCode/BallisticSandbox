using System;
using BallisticSandbox.Infrastructure.DI.Bind;
using BallisticSandbox.Infrastructure.DI.Collection;
using BallisticSandbox.Infrastructure.DI.Provider;
using BallisticSandbox.Infrastructure.DI.Exceptions;
using BallisticSandbox.Infrastructure.DI.Injection;

namespace BallisticSandbox.Infrastructure.DI.Container
{
    public class DIContainer : IDependencyRegistry, IDependencyResolver, IDependencyContainer
    {
        private readonly IBindingCollection _bindingCollection;
        private readonly IInstanceProvider _instanceProvider;
        private readonly IDependencyInjector _dependencyInjector;
        private readonly IDependencyContainer _parentContainer;
        private readonly BindingBuilder _bindingBuilder;

        public DIContainer(IDependencyContainer parent)
        {
            _parentContainer = parent;

            _bindingCollection = new BindingCollection();
            _instanceProvider = new InstanceProvider(this, _bindingCollection);
            _dependencyInjector = new DependencyInjector(this);
            _bindingBuilder = new BindingBuilder(this);
        }

        public IBindingContractState SetupBinding()
        {
            return _bindingBuilder.SetupBinding();
        }

        public void Register(BindData bindData)
        {
            if (bindData == null)
                throw new ArgumentNullException(nameof(bindData));

            try
            {
                _bindingCollection.Add(bindData);
            }
            catch (Exception ex)
            {
                throw new BindingRegistrationException("Binding registration failed", bindData, ex);
            }
        }

        public bool IsRegistered(Type contractType, object identifier = null)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType), "Contract type cannot be null.");

            return _bindingCollection.Contains(contractType, identifier);
        }

        public object Resolve(Type contractType, object identifier = null)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType), "Contract type cannot be null.");

            try
            {
                if (_instanceProvider.TryGetInstance(contractType, identifier, out object instance))
                    return instance;

                if (_parentContainer != null)
                    return _parentContainer.Resolve(contractType, identifier);

                throw new BindingNotFoundException($"No binding found for {contractType.Name} with identificator {identifier}.", contractType);
            }
            catch (BindingNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DependencyResolutionException($"Container failed to resolve {contractType.Name}.", contractType, null, ex);
            }
        }

        public T Resolve<T>(Type contractType, object identifier = null)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType), "Contract type cannot be null.");

            if (typeof(T).IsAssignableFrom(contractType))
                throw new InvalidOperationException("Implementation type must be assignable to the contract type.");

            return (T)Resolve(contractType, identifier);
        }

        public void Inject(object instance, InjectionType injectionType)
        {
            _dependencyInjector.Inject(instance, injectionType);
        }
    }
}
