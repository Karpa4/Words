using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public interface IInputProvider
    {
        Vector2 PointerPosition { get; }
    
        event Action PointerUpEvent;
        event Action PointerDownEvent;
    
        void Initialize();
        void Stop();
        void Dispose();
    }

    public class InputProvider : IInputProvider
    {
        private InputActions _inputActions;

        public Vector2 PointerPosition { get; private set; }

        public event Action PointerUpEvent;
        public event Action PointerDownEvent;
        
        public void Initialize()
        {
            _inputActions ??= new InputActions();
            _inputActions.UI.Point.started += OnPointerStarted;
            _inputActions.UI.Point.performed += OnPointerPerformed;
            _inputActions.UI.Point.canceled += OnPointerFinished;
            _inputActions.UI.Point.Enable();
        }

        private void OnPointerStarted(InputAction.CallbackContext context)
        {
            PointerPosition = context.ReadValue<Vector2>();
            PointerDownEvent?.Invoke();
        }
        
        private void OnPointerPerformed(InputAction.CallbackContext context)
        {
            PointerPosition = context.ReadValue<Vector2>();
        }
        
        private void OnPointerFinished(InputAction.CallbackContext context)
        {
            PointerUpEvent?.Invoke();
        }

        public void Stop()
        {
            _inputActions.UI.Point.started -= OnPointerStarted;
            _inputActions.UI.Point.performed -= OnPointerPerformed;
            _inputActions.UI.Point.canceled -= OnPointerFinished;
            _inputActions.UI.Point.Disable();
        }

        public void Dispose()
        {
            _inputActions?.Dispose();
            _inputActions = null;
        }
    }
}