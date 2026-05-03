using System;

namespace BallisticSandbox.Infrastructure.DI.Bind
{
    public interface IBindingContractState
    {
        IBindingImplementationState Bind<TContract>() where TContract : class;
        IBindingImplementationState Bind(Type contractType);
    }
}
