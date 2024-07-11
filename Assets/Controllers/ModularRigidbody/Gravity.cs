using UnityEngine;

namespace Controllers.ModularRigidbody {
    public class Gravity : MonoBehaviour, IVector {
        public Vector3 Value { get; private set; }

        [SerializeField] VelocityApplier velocityApplier;
        [SerializeField] GroundChecker groundChecker;

        [SerializeField] private bool enableGroundCheck = false;
        [SerializeField] private bool gravityEnable = true;

        public float Force { get { return gravityForce; } }
        [SerializeField] private float gravityForce = 10;
        [SerializeField] private float gravityScale = 1;

        private void Awake() {
            velocityApplier = GetComponent<VelocityApplier>();
        }
        private void OnEnable() => velocityApplier.AddForce(this);
        private void OnDisable() => velocityApplier.RemoveForce(this);

        private void FixedUpdate() {
            Value = CalculateGravity();
        }

        Vector3 CalculateGravity() {
            if (!gravityEnable) return Vector3.zero;

            if (groundChecker.IsGrounded && enableGroundCheck) {
                return Vector3.zero;
            }

            if (!gravityEnable) {
                return Vector3.zero;
            }

            Vector3 gravity = Vector3.down * gravityForce * gravityScale;
            return gravity;
        }

        public void SetGravityScale(float value) => gravityScale = value;
        public void SetGravityForce(float value) => gravityForce = value;
        public void SetGravityState(bool state = true) => gravityEnable = state;

    }

}