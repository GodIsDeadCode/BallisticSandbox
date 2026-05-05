using System;
using System.Collections.Generic;
using UnityEngine;
using BallisticSandbox.Infrastructure.DI.Bind;
using BallisticSandbox.Infrastructure.DI.Collection;
using BallisticSandbox.Infrastructure.DI.Container;
using BallisticSandbox.Infrastructure.DI.Exceptions;
using BallisticSandbox.Infrastructure.DI.Factory;

namespace BallisticSandbox.Infrastructure.DI.Provider
{
    public class InstanceProvider : IInstanceProvider
    {
        private readonly Stack<Type> _resolutionStack;
        private readonly HashSet<Type> _currentlyResolvingTypes;

        private readonly IDependencyResolver _dependencyResolver;
        private readonly IBindingCollection _bindingCollection;
        private readonly IInstanceFactory _instanceFactory;

        private readonly Dictionary<BindID, object> _singletonInstances;
        private readonly Dictionary<BindID, object> _scopedInstances;
        private readonly object _singletonLock = new();
        private readonly object _scopedLock = new();

        public InstanceProvider(IDependencyResolver dependencyResolver, IBindingCollection bindingCollection)
        {
            _dependencyResolver = dependencyResolver;
            _bindingCollection = bindingCollection;

            _instanceFactory = new InstanceFactory(dependencyResolver);
            _singletonInstances = new Dictionary<BindID, object>();
            _scopedInstances = new Dictionary<BindID, object>();
            _resolutionStack = new Stack<Type>();
            _currentlyResolvingTypes = new HashSet<Type>();
        }

        public bool TryGetInstance(Type contractType, object identifier, out object instance)
        {
            try
            {
                instance = GetInstance(contractType, identifier);
                return true;
            }
            catch (BindingNotFoundException)
            {
                instance = null;
                return false;
            }
            catch (DependencyResolutionException)
            {
                instance = null;
                return false;
            }
        }

        public T GetInstance<T>(object identifier = null)
        {
            return (T)GetInstance(typeof(T), identifier);
        }

        public object GetInstance(Type contractType, object identifier = null)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType), "Contract type cannot be null.");

            if (!_bindingCollection.TryGet(contractType, identifier, out BindData bindData))
                throw new BindingNotFoundException("No binding found for the specified contract type and identifier.", contractType, null, identifier);

            if (bindData.Lifetime == Lifetime.Singleton)
                return GetSingletonInstance(bindData);

            if (bindData.Lifetime == Lifetime.Scoped)
                return GetScopedInstance(bindData);

            return GetInstanceInternal(bindData);
        }

        private object GetScopedInstance(BindData bindData)
        {
            BindID bindId = new(bindData.ContractType, bindData.Identifier);
            if (_scopedInstances.TryGetValue(bindId, out object scopedInstance))
                return scopedInstance;

            lock (_scopedLock)
            {
                if (_scopedInstances.TryGetValue(bindId, out scopedInstance))
                    return scopedInstance;

                scopedInstance = GetInstanceInternal(bindData);
                _scopedInstances[bindId] = scopedInstance;

                return scopedInstance;
            }
        }

        private object GetSingletonInstance(BindData bindData)
        {
            BindID bindID = new(bindData.ContractType, bindData.Identifier);
            if (_singletonInstances.TryGetValue(bindID, out object singletonInstance))
                return singletonInstance;

            lock (_singletonLock)
            {
                if (_singletonInstances.TryGetValue(bindID, out singletonInstance))
                    return singletonInstance;

                singletonInstance = GetInstanceInternal(bindData);
                _singletonInstances[bindID] = singletonInstance;

                return singletonInstance;
            }
        }

        private object GetInstanceInternal(BindData bindData)
        {
            Type impType = bindData.ImplementationType;
            if (IsCurrentlyResolving(impType))
                throw new DependencyResolutionException("Circular dependency detected", impType, _resolutionStack, null);

            EnterResolution(impType);
            try
            {
                if (bindData.Instance != null)
                    return bindData.Instance;

                if (bindData.Factory != null)
                    return bindData.Factory.Invoke(_dependencyResolver) ?? throw new InvalidOperationException($"Factory for type {impType} returned null");

                return _instanceFactory.CreateInstance(impType, bindData.Arguments);
            }
            catch (DependencyResolutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                throw new DependencyResolutionException($"Failed to resolve type {impType}: {ex.Message}", impType, _resolutionStack, ex);
            }
            finally
            {
                ExitResolution(impType);
            }
        }

        private bool IsCurrentlyResolving(Type type) => _currentlyResolvingTypes.Contains(type);

        private void EnterResolution(Type type)
        {
            _resolutionStack.Push(type);
            _currentlyResolvingTypes.Add(type);
        }

        private void ExitResolution(Type type)
        {
            _resolutionStack.Pop();
            _currentlyResolvingTypes.Remove(type);
        }
    }
}
