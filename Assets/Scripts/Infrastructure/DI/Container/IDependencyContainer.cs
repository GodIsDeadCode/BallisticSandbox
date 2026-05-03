using BallisticSandbox.Infrastructure.DI.Bind;
using BallisticSandbox.Infrastructure.DI.Injection;
using System;

namespace BallisticSandbox.Infrastructure.DI.Container
{
    public interface IDependencyContainer
    {
        void Inject(object instance, InjectionType injectionType);
        bool IsRegistered(Type contractType, object identifier = null);
        void Register(BindData bindData);
        object Resolve(Type contractType, object identifier = null);
        T Resolve<T>(Type contractType, object identifier = null);
        IBindingContractState SetupBinding();
    }
}