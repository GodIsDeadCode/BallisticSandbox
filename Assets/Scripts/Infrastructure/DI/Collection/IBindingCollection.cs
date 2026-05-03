using BallisticSandbox.Infrastructure.DI.Bind;
using System;

namespace BallisticSandbox.Infrastructure.DI.Collection
{
    public interface IBindingCollection
    {
        void Add(BindData bindData);
        void Clear();
        bool Contains(Type contracType, object identifier);
        BindData Get(Type contractType, object identifier = null);
        bool TryAdd(BindData bindData);
        bool TryGet(Type contractType, object identifier, out BindData bindData);
    }
}