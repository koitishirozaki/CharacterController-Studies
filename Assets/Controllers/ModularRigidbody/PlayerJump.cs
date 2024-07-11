using Controllers.Input;
using UnityEngine;

namespace Controllers.ModularRigidbody {
    public class PlayerJump : MonoBehaviour, IVector {
        public Vector3 Value { get; private set; }

        PlayerInputHandler inputHandler;
        VelocityApplier velocityApplier;
        GroundChecker groundChecker;
        Gravity gravity;
        ForceReceiver forceReceiver;

        [SerializeField] private float jumpForce = 20;
        [SerializeField] private float gravityScaleDown = 3;
        [SerializeField] private float gravityScaleGround = 0.2f;

        private void Awake() {
            forceReceiver = GetComponent<ForceReceiver>();
            velocityApplier = GetComponent<VelocityApplier>();
            groundChecker = GetComponent<GroundChecker>();
            inputHandler = GetComponent<PlayerInputHandler>();
            gravity = GetComponent<Gravity>();
        }
        private void OnEnable() => velocityApplier.AddForce(this);
        private void OnDisable() => velocityApplier.RemoveForce(this);

        private void FixedUpdate() {
            JumpHandler();

            if (inputHandler.JumpPress) {
                Jump();
                inputHandler.ResetJump();
            }
        }

        public void Jump() {
            if (!groundChecker.IsGrounded) return;
            Debug.Log("Jump");
            forceReceiver.AddInstantForce(Vector3.up * jumpForce);
        }

        void JumpHandler() {
            if (groundChecker.IsGrounded) {
                gravity.SetGravityScale(gravityScaleGround);
            } else {
                if (velocityApplier.rb.velocity.y < 0.5f) {
                    gravity.SetGravityScale(gravityScaleDown);
                }
            }
        }


    }
}