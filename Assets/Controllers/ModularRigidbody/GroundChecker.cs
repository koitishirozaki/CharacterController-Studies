using MyBox;
using UnityEngine;

namespace Controllers.ModularRigidbody {
    public class GroundChecker : MonoBehaviour {
        public bool IsGrounded { get { return _isGrounded; } }
        [SerializeField, ReadOnly] private bool _isGrounded;
        public Vector3 GroundNormal { get { return _groundNormal; } }
        [SerializeField, ReadOnly] private Vector3 _groundNormal;

        [Range(0.01f, 0.5f), SerializeField] private float detectionRange = 0.08f;
        [SerializeField] private Transform detectionCenter;
        [SerializeField] private Vector3 detectionSize = Vector3.one;

        [SerializeField] private LayerMask groundMask;

        private void FixedUpdate() {
            _isGrounded = CheckForGround();
            _groundNormal = GetGroundNormal();
        }

        bool CheckForGround() {
            Collider[] colliders = Physics.OverlapBox(detectionCenter.position, detectionSize, transform.rotation, groundMask);
            if (colliders.Length > 0) return true;
            return false;
        }
        Vector3 GetGroundNormal() {
            if (!_isGrounded) return Vector3.up;
            if (Physics.Raycast(detectionCenter.position, Vector3.down, out RaycastHit hitInfo, detectionSize.y, groundMask)) {
                return hitInfo.normal;
            }
            return Vector3.down;
        }

        private void OnDrawGizmos() {
            if (detectionCenter == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawCube(detectionCenter.position, detectionSize);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(detectionCenter.position, detectionCenter.position + Vector3.down * detectionSize.y);
        }
    }
}