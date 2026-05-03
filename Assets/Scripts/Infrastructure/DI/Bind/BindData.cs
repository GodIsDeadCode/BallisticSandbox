using System;
using System.Collections.Generic;
using BallisticSandbox.Infrastructure.DI.Container;
using BallisticSandbox.Infrastructure.DI.Utility;

namespace BallisticSandbox.Infrastructure.DI.Bind
{
    public class BindData
    {
        public readonly IReadOnlyDictionary<int, TypeValuePair> Arguments;
        public readonly Type ContractType;
        public readonly Type ImplementationType;
        public readonly Func<IDependencyResolver, object> Factory;
        public readonly object Instance;
        public readonly object Identifier;
        public readonly Lifetime Lifetime;

        public BindData(
            IReadOnlyDictionary<int, TypeValuePair> arguments, 
            Type contractType, 
            Type implementationType, 
            Func<IDependencyResolver, object> factory, 
            object instance, 
            object identifier, 
            Lifetime lifetime)
        {
            Arguments = arguments;
            ContractType = contractType;
            ImplementationType = implementationType;
            Factory = factory;
            Instance = instance;
            Identifier = identifier;
            Lifetime = lifetime;
        }
    }

    public enum Lifetime
    {
        Transient,
        Singleton,
        Scoped
    }
}
