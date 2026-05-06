using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BallisticSandbox.Infrastructure.Services.Input
{
    public class InputService : IDisposable, IInputService
    {
        private readonly InputSystemActions _inputActions;
        private bool _isDisposed;

        public event Action<Vector2> OnMovePerformedCallback;
        public event Action<Vector2> OnMoveCanceledCallback;
        public event Action<Vector2> OnLookPerformedCallback;
        public event Action<Vector2> OnLookCanceledCallback;
        public event Action OnJumpStartedCallback;
        public event Action OnJumpCanceledCallback;
        public event Action OnSprintPerformedCallback;
        public event Action OnSprintCanceledCallback;
        public event Action OnCrouchStartedCallback;
        public event Action OnCrouchCanceledCallback;
        public event Action OnProneStartedCallback;
        public event Action OnProneCanceledCallback;

        public Vector2 MoveData { get; private set; }
        public Vector2 LookData { get; private set; }
        public bool IsJumpPress { get; private set; }
        public bool IsSprintPress { get; private set; }
        public bool IsCrouchPress { get; private set; }
        public bool IsPronePress { get; private set; }

        public InputService()
        {
            _inputActions = new InputSystemActions();
        }

        public void Enable()
        {
            _inputActions.Enable();
            SubscribeInputActions();
        }

        public void Disable()
        {
            UnsubscribeInputActions();
            _inputActions.Disable();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void SubscribeInputActions()
        {
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;
            _inputActions.Player.Look.performed += OnLookPerformed;
            _inputActions.Player.Look.canceled += OnLookCanceled;
            _inputActions.Player.Jump.started += OnJumpStarted;
            _inputActions.Player.Jump.canceled += OnJumpCanceled;
            _inputActions.Player.Sprint.performed += OnSprintPerformed;
            _inputActions.Player.Sprint.canceled += OnSprintCanceled;
            _inputActions.Player.Crouch.started += OnCrouchStarted;
            _inputActions.Player.Crouch.canceled += OnCrouchCanceled;
            _inputActions.Player.Prone.started += OnProneStarted;
            _inputActions.Player.Prone.canceled += OnProneCanceled;
        }

        private void UnsubscribeInputActions()
        {
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            _inputActions.Player.Look.performed -= OnLookPerformed;
            _inputActions.Player.Look.canceled -= OnLookCanceled;
            _inputActions.Player.Jump.started -= OnJumpStarted;
            _inputActions.Player.Jump.canceled -= OnJumpCanceled;
            _inputActions.Player.Sprint.performed -= OnSprintPerformed;
            _inputActions.Player.Sprint.canceled -= OnSprintCanceled;
            _inputActions.Player.Crouch.started -= OnCrouchStarted;
            _inputActions.Player.Crouch.canceled -= OnCrouchCanceled;
        }

        private void OnCrouchCanceled(InputAction.CallbackContext context)
        {
            IsCrouchPress = false;
            OnCrouchCanceledCallback?.Invoke();
        }

        private void OnCrouchStarted(InputAction.CallbackContext context)
        {
            IsCrouchPress = true;
            OnCrouchStartedCallback?.Invoke();
        }

        private void OnProneCanceled(InputAction.CallbackContext context)
        {
            IsPronePress = false;
            OnProneCanceledCallback?.Invoke();
        }

        private void OnProneStarted(InputAction.CallbackContext context)
        {
            IsPronePress = true;
            OnProneStartedCallback?.Invoke();
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            IsSprintPress = false;
            OnSprintCanceledCallback?.Invoke();
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            IsSprintPress = true;
            OnSprintPerformedCallback?.Invoke();
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            IsJumpPress = false;
            OnJumpCanceledCallback?.Invoke();
        }

        private void OnJumpStarted(InputAction.CallbackContext context)
        {
            IsJumpPress = true;
            OnJumpStartedCallback?.Invoke();
        }

        private void OnLookCanceled(InputAction.CallbackContext context)
        {
            LookData = Vector2.zero;
            OnLookCanceledCallback?.Invoke(LookData);
        }

        private void OnLookPerformed(InputAction.CallbackContext context)
        {
            LookData = context.ReadValue<Vector2>();
            OnLookPerformedCallback?.Invoke(LookData);
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            MoveData = Vector2.zero;
            OnMoveCanceledCallback?.Invoke(MoveData);
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            MoveData = context.ReadValue<Vector2>();
            OnMovePerformedCallback?.Invoke(MoveData);
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Disable();
                    _inputActions.Dispose();
                }

                _isDisposed = true;
            }
        }
    }
}
