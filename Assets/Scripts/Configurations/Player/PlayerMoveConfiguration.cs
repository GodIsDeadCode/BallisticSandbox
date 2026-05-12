using System;
using UnityEngine;
using BallisticSandbox.Configurations.Base;

namespace BallisticSandbox.Configurations.Player
{
    [CreateAssetMenu(fileName = "PlayerMoveConfiguration", menuName = "Configurations/Player/MoveConfiguration")]
    public class PlayerMoveConfiguration : Configuration
    {
        [Header("Move Parameters")]
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _sprintSpeed;
        [SerializeField] private float _moveAcceleration;
        [SerializeField] private float _jumpHeight;
        [SerializeField][Range(0.01f, 3f)] private float _fallThresholdTime;

        [Header("Slide Parameters")]
        [SerializeField] private float _slideMaxSpeed;
        [SerializeField] private float _slideAcceleration;
        [SerializeField][Range(10f, 90f)] private float _slideAngleThreshold;

        [Header("Gravity Parameters")]
        [SerializeField][Range(1f, 5f)] private float _stickToGroundForce;
        [SerializeField][Range(1f, 5f)] private float _gravityMultiplier;

        public float WalkSpeed => _walkSpeed;
        public float SprintSpeed => _sprintSpeed;
        public float MoveAcceleration => _moveAcceleration;
        public float JumpHeight => _jumpHeight;
        public float FallThresholdTime => _fallThresholdTime;
        public float SlideMaxSpeed => _slideMaxSpeed;
        public float SlideAcceleration => _slideAcceleration;
        public float SlideAngleThreshold => _slideAngleThreshold;
        public float StickToGroundForce => _stickToGroundForce;
        public float GravityMultiplier => _gravityMultiplier;
    }
}
