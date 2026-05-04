
namespace BallisticSandbox.Infrastructure.DI.Container
{
    public interface IDependencyResolver
    {
        object Resolve(System.Type contractType, object identifier = null);
        object Resolve<TContract>(object identifier = null);
        TImplementation Resolve<TContract, TImplementation>(object identifier = null) where TImplementation : class;
    }
}
