using System;
using System.Collections.Generic;
using BallisticSandbox.Infrastructure.DI.Container;
using BallisticSandbox.Infrastructure.DI.Utility;

namespace BallisticSandbox.Infrastructure.DI.Bind
{
    public class BindingBuilderContext
    {
        public readonly Dictionary<int, TypeValuePair> Arguments = new();

        public Type ContractType;
        public Type ImplementationType;
        public Func<IDependencyResolver, object> Factory;
        public object Instance;
        public object Identifier;
        public Lifetime Lifetime;

        public void Reset()
        {
            Arguments.Clear();

            ContractType = null;
            ImplementationType = null;
            Factory = null;
            Instance = null;
            Identifier = null;
            Lifetime = Lifetime.Transient;
        }

        public BindData BuildBindData()
        {
            return new BindData(
                Arguments.Count > 0 ? new Dictionary<int, TypeValuePair>(Arguments) : null,
                ContractType,
                ImplementationType,
                Factory,
                Instance,
                Identifier,
                Lifetime);
        }
    }
}
