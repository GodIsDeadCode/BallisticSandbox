using System.Collections.Generic;
using UnityEngine;
using BallisticSandbox.Infrastructure.DI.Installer;
using BallisticSandbox.Infrastructure.DI.Container;

namespace BallisticSandbox.Infrastructure.DI.Context
{
    public abstract class Context : MonoBehaviour
    {
        [SerializeField] private List<MonoInstaller> _monoInstallers;
        [SerializeField] private List<ScriptableObjectInstaller> _scriptableObjectInstallers;

        public IDependencyContainer DependencyContainer { get; protected set; }

        protected virtual void InitContainer()
        {
            if (DependencyContainer == null)
                DependencyContainer = new DIContainer(null);
        }

        protected virtual void InstallBindings()
        {
            if (_monoInstallers != null)
            {
                for (int i = 0; i < _monoInstallers.Count; i++)
                {
                    if (_monoInstallers[i] == null)
                    {
                        Debug.LogError($"[{GetType().Name}] MonoInstaller at index {i} is null. " +
                                       $"Check the inspector on '{gameObject.name}'.", this);
                        continue;
                    }

                    _monoInstallers[i].InstallBindings(DependencyContainer);
                }
            }

            if (_scriptableObjectInstallers != null)
            {
                for (int i = 0; i < _scriptableObjectInstallers.Count; i++)
                {
                    if (_scriptableObjectInstallers[i] == null)
                    {
                        Debug.LogError($"[{GetType().Name}] ScriptableObjectInstaller at index {i} is null. " +
                                       $"Check the inspector on '{gameObject.name}'.", this);
                        continue;
                    }

                    _scriptableObjectInstallers[i].InstallBindings(DependencyContainer);
                }
            }
        }

    }
}
