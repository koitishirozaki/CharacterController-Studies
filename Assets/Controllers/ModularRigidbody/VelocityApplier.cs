using System.Collections.Generic;
using UnityEngine;

namespace Controllers.ModularRigidbody {
    public class VelocityApplier : MonoBehaviour {
        public Rigidbody rb { get; private set; }

        [Range(0, 0.3f), SerializeField] private float smoothMovement = 0.05f;

        public readonly List<IVector> modifiers = new List<IVector>();
        public readonly List<IVector> forces = new List<IVector>();

        private Vector3 movementVector = Vector3.zero;
        private Vector3 forceVector = Vector3.zero;

        public void AddForce(IVector v) => forces.Add(v);
        public void RemoveForce(IVector v) => forces.Remove(v);
        public void AddModifier(IVector v) => modifiers.Add(v);
        public void RemoveModifier(IVector v) => modifiers.Remove(v);

        private void Awake() {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate() {
            ApplyForcesAndModifiers();
        }

        void ApplyForcesAndModifiers() {
            movementVector = Vector3.zero;
            forceVector = Vector3.zero;

            foreach (IVector vector in modifiers) {
                movementVector += vector.Value;
            }
            movementVector = Vector3.SmoothDamp(rb.velocity, movementVector, ref movementVector, smoothMovement);

            foreach (IVector vector in forces) {
                forceVector += vector.Value;
            }
            rb.velocity = movementVector + forceVector;
        }
    }

}