using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

namespace Controllers.Input {
    public class PlayerInputHandler : MonoBehaviour {
        private InputBindings input;

        public bool InputEnabled { get; private set; }

        public Vector2 RawDirection { get { return _rawDirection; } }
        [SerializeField, ReadOnly] private Vector2 _rawDirection;
        public bool JumpPress { get { return _jumpPress; } }
        [SerializeField, ReadOnly] private bool _jumpPress;

        private void Awake() {
            input = new InputBindings();
            // Add new actions here
            input.Player.Move.performed += onMove;
            input.Player.Move.canceled += onMove;

            input.Player.Jump.started += onJump;
            input.Player.Jump.canceled += onJump;

            EnableInput();
        }

        public void EnableInput(bool state = true) {
            InputEnabled = state;
            if (state) {
                input.Enable();
            } else {
                input.Disable();
            }
        }
        private void onMove(InputAction.CallbackContext ctx) {
            _rawDirection = ctx.ReadValue<Vector2>();
        }
        private void onJump(InputAction.CallbackContext ctx) {
            _jumpPress = ctx.ReadValueAsButton();
        }
        public void ResetJump() => _jumpPress = false;
    }
}
