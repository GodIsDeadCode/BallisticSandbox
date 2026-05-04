using BallisticSandbox.Infrastructure.DI.Installer;
using BallisticSandbox.Infrastructure.DI.Container;

namespace BallisticSandbox
{
    public class Installer : MonoInstaller
    {
        public override void InstallBindings(IDependencyContainer dependencyContainer)
        {
            dependencyContainer.SetupBinding().Bind<IInputService>().To<InputService>().AsSingleton().CommitBinding();
            dependencyContainer.SetupBinding().Bind<IInputService>().To<InputService>().AsSingleton().WithIdentifier("A").CommitBinding();
        }
    }
}
