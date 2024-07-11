using UnityEngine;

namespace Controllers.ModularRigidbody {

    [RequireComponent(typeof(VelocityApplier))]
    public class ForceReceiver : MonoBehaviour, IVector {
        public Vector3 Value { get; private set; }

        private VelocityApplier velocityApplier;

        [SerializeField] private float drag = 1;
        [SerializeField] private float mass = 1;
        [SerializeField] private float instantDecayMult = 4;

        private Vector3 decayForce;
        private Vector3 instantForce;

        private void Awake() {
            velocityApplier = GetComponent<VelocityApplier>();
        }

        private void OnEnable() => velocityApplier.AddForce(this);
        private void OnDisable() => velocityApplier.RemoveForce(this);

        private void FixedUpdate() {
            instantForce = Vector3.zero;
            HandleDecayingForces();
            HandleInstantForces();
            Value = decayForce + instantForce;
        }

        private void HandleInstantForces() {
            if (instantForce.magnitude < 0.2f) instantForce = Vector3.zero;
        }

        private void HandleDecayingForces() {
            if (decayForce.magnitude < 0.2f) decayForce = Vector3.zero;
            decayForce = Vector3.Lerp(decayForce, Vector3.zero, drag * Time.deltaTime);
        }

        public void AddForce(Vector3 force) {
            decayForce += force / mass;
        }
        public void AddInstantForce(Vector3 force) {
            instantForce += force / mass;
        }
    }
}