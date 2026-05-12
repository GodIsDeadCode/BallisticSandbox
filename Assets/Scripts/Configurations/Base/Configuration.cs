using UnityEngine;

namespace BallisticSandbox.Configurations.Base
{
    public abstract class Configuration : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private string _id;
        [SerializeField] private string _domain;

        public string ID => _id;
        public string Domain => _domain;
    }
}
