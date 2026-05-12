using System;
using UnityEngine;
using BallisticSandbox.Configurations.Base;

namespace BallisticSandbox.Configurations.Player
{
    [CreateAssetMenu(fileName = "PlayerLookConfiguration", menuName = "Configurations/Player/LookConfiguration")]
    public class PlayerLookConfiguration : Configuration
    {
        [Header("Look Parameters")]
        [SerializeField][Range(0f, 5f)] private float _sensitivityX;
        [SerializeField][Range(0f, 5f)] private float _sensitivityY;
        [SerializeField][Range(-90f, 35f)] private float _minAngleX;
        [SerializeField][Range(35f, 90f)] private float _maxAngleX;

        public float SensitivityX => _sensitivityX;
        public float SensitivityY => _sensitivityY;
        public float MinAngleX => _minAngleX;
        public float MaxAngleX => _maxAngleX;
    }
}
