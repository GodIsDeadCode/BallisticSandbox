using System;
using UnityEngine;

namespace BallisticSandbox.Infrastructure.DI.Context
{
    [DefaultExecutionOrder(-10000)]
    public class ProjectContext : Context
    {
        public static ProjectContext Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                DestroyImmediate(Instance);

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitContainer();
            InstallBindings();
        }
    }
}
