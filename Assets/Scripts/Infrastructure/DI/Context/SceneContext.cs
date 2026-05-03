using BallisticSandbox.Infrastructure.DI.Container;
using UnityEngine;

namespace BallisticSandbox.Infrastructure.DI.Context
{
    [DefaultExecutionOrder(-50000)]
    public class SceneContext : Context
    {
        public static SceneContext Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                DestroyImmediate(Instance);

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitContainer();
            InstallBindings();
        }

        protected override void InitContainer()
        {
            if (ProjectContext.Instance == null)
            {
                Debug.LogWarning("[SceneContext] ProjectContext not found. " +
                                 "SceneContext will run as a standalone root container. " +
                                 "Add a ProjectContext to your first scene if this is unintended.", this);

                DependencyContainer = new DIContainer(null);
            }
            else
            {
                DependencyContainer = new DIContainer(ProjectContext.Instance.DependencyContainer);
            }
        }
    }
}
