using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallisticSandbox.Configurations.Base
{
    [CreateAssetMenu(fileName = "ConfigrationDataBase", menuName = "Configurations/DataBase/ConfigurationDataBase")]
    public class ConfigurationDataBase : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private List<Configuration> _configurations;

        public IReadOnlyList<Configuration> Configurations => _configurations;
    }
}
