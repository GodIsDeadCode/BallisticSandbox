using BallisticSandbox.Infrastructure.DI.Container;

namespace BallisticSandbox.Infrastructure.DI.Installer
{
    public interface IInstaller
    {
        void InstallBindings(IDependencyContainer dependencyContainer);
    }
}
