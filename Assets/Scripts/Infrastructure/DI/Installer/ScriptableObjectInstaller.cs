using BallisticSandbox.Infrastructure.DI.Container;
using UnityEngine;

namespace BallisticSandbox.Infrastructure.DI.Installer
{
    public abstract class ScriptableObjectInstaller : MonoBehaviour, IInstaller
    {
        public abstract void InstallBindings(IDependencyContainer dependencyContainer);
    }
}
