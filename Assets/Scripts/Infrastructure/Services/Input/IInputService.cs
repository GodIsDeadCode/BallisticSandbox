using System;
using UnityEngine;

namespace BallisticSandbox.Infrastructure.Services.Input
{
    public interface IInputService
    {
        bool IsJumpPress { get; }
        bool IsSprintPress { get; }
        Vector2 LookData { get; }
        Vector2 MoveData { get; }
        bool IsCrouchPress { get; }
        bool IsPronePress { get; }

        event Action OnJumpCanceledCallback;
        event Action OnJumpStartedCallback;
        event Action<Vector2> OnLookCanceledCallback;
        event Action<Vector2> OnLookPerformedCallback;
        event Action<Vector2> OnMoveCanceledCallback;
        event Action<Vector2> OnMovePerformedCallback;
        event Action OnSprintCanceledCallback;
        event Action OnSprintPerformedCallback;
        event Action OnProneCanceledCallback;
        event Action OnProneStartedCallback;
        event Action OnCrouchCanceledCallback;
        event Action OnCrouchStartedCallback;

        void Disable();
        void Dispose();
        void Enable();
    }
}