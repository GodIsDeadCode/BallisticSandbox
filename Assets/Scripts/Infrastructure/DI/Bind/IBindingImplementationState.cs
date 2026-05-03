using System;
using BallisticSandbox.Infrastructure.DI.Container;

namespace BallisticSandbox.Infrastructure.DI.Bind
{
    public interface IBindingImplementationState
    {
        IBindingLifetimeState To<TImplementation>() where TImplementation : class;
        IBindingLifetimeState To(Type implementationType);
        IBindingLifetimeState ToFactory(Func<IDependencyResolver, object> factory);
        IBindingLifetimeState ToInstance(object instance);
        IBindingLifetimeState ToSelf();
    }
}
