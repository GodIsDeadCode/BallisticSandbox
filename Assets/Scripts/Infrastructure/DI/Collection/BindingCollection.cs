using System;
using System.Collections.Concurrent;
using BallisticSandbox.Infrastructure.DI.Bind;

namespace BallisticSandbox.Infrastructure.DI.Collection
{
    public class BindingCollection : IBindingCollection
    {
        private readonly ConcurrentDictionary<BindID, BindData> _bindings;

        public BindingCollection()
        {
            _bindings = new ConcurrentDictionary<BindID, BindData>();
        }

        public void Add(BindData bindData)
        {
            if (bindData == null)
                throw new ArgumentNullException(nameof(bindData), "Bind data cannot be null.");

            BindID bindID = new BindID(bindData.ContractType, bindData.Identifier);
            if (_bindings.ContainsKey(bindID))
                throw new InvalidOperationException($"A binding for contract type {bindData.ContractType} with identifier {bindData.Identifier} already exists.");

            _bindings[bindID] = bindData;
        }

        public bool TryAdd(BindData bindData)
        {
            if (bindData == null)
                throw new ArgumentNullException(nameof(bindData), "Bind data cannot be null.");

            BindID bindID = new BindID(bindData.ContractType, bindData.Identifier);
            if (_bindings.ContainsKey(bindID))
                return false;

            _bindings[bindID] = bindData;
            return true;
        }

        public BindData Get(Type contractType, object identifier = null)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType), "Contract type cannot be null.");

            BindID bindID = new BindID(contractType, identifier);
            if (_bindings.TryGetValue(bindID, out BindData bindData))
                return bindData;

            throw new InvalidOperationException($"No binding found for contract type {contractType} with identifier {identifier}.");
        }

        public bool TryGet(Type contractType, object identifier, out BindData bindData)
        {
            if (contractType == null)
                throw new ArgumentNullException(nameof(contractType), "Contract type cannot be null.");

            BindID bindID = new BindID(contractType, identifier);
            return _bindings.TryGetValue(bindID, out bindData);
        }

        public bool Contains(Type contracType, object identifier)
        {
            if (contracType == null)
                throw new ArgumentNullException(nameof(contracType), "Contract type cannot be null.");

            return _bindings.ContainsKey(new BindID(contracType, identifier));
        }

        public void Clear()
        {
            _bindings.Clear();
        }
    }
}
