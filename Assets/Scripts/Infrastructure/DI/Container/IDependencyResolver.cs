
namespace BallisticSandbox.Infrastructure.DI.Container
{
    public interface IDependencyResolver
    {
        object Resolve(System.Type contractType, object identifier = null);
        T Resolve<T>(System.Type contractType, object identifier = null);
    }
}
