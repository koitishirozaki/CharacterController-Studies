using Controllers.Input;
using UnityEngine;

namespace Controllers.ModularRigidbody {
    public class PlayerMovement : MonoBehaviour, IVector {
        public Vector3 Value { get; private set; }

        PlayerInputHandler input;
        VelocityApplier velocityApplier;
        GroundChecker groundChecker;
        Camera cam;

        [SerializeField] private float movespeed = 10;
        [Range(0, 0.5f), SerializeField] private float smoothAirDampen = 0.03f;

        private Vector2 inputDirection = Vector2.zero;

        private void Awake() {
            velocityApplier = GetComponent<VelocityApplier>();
            groundChecker = GetComponent<GroundChecker>();
            input = GetComponent<PlayerInputHandler>();
            cam = Camera.main;
        }
        private void OnEnable() => velocityApplier.AddForce(this);
        private void OnDisable() => velocityApplier.RemoveForce(this);

        void FixedUpdate() {
            if (input.RawDirection.magnitude == 0) {
                Value = Vector3.zero;
                return;
            }

            Vector3 direction = ConvertInputWorldSpace(input.RawDirection);
            direction = Vector3.Cross(direction, Vector3.up).normalized;
            Vector3 transformedDirection = Vector3.Cross(groundChecker.GroundNormal, direction);

            transformedDirection *= movespeed;
            Value = transformedDirection;
        }

        Vector3 ConvertInputWorldSpace(Vector2 input) {
            Vector3 worldSpace = new Vector3(input.x, 0, input.y).normalized;
            if (!cam) {
                Debug.LogError("Camera could not be found. Cant convert input to World Space, fallback to input space");
                return worldSpace;
            }
            worldSpace = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * worldSpace;
            return worldSpace;
        }
    }
}