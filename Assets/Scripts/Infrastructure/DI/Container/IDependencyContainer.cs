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
        object Resolve<TContract>(object identifier = null);
        TImplementation Resolve<TContract, TImplementation>(object identifier = null) where TImplementation : class;
        IBindingContractState SetupBinding();
    }
}