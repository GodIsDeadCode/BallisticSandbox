using BallisticSandbox.Infrastructure.DI.Bind;
using System;
using System.Collections.Generic;

namespace BallisticSandbox.Infrastructure.DI.Collection
{
    public interface IBindingCollection
    {
        void Add(BindData bindData);
        void Clear();
        bool Contains(Type contracType, object identifier);
        BindData Get(Type contractType, object identifier = null);
        IReadOnlyCollection<BindData> GetAllRegisteredBinds();
        bool TryAdd(BindData bindData);
        bool TryGet(Type contractType, object identifier, out BindData bindData);
    }
}