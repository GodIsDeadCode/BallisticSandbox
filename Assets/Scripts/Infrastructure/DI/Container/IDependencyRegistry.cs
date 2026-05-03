
namespace BallisticSandbox.Infrastructure.DI.Container
{
    public interface IDependencyRegistry
    {
        void Register(Bind.BindData bindData);
        bool IsRegistered(System.Type contractType, object identifier = null);
    }
}
