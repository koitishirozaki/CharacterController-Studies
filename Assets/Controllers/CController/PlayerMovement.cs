using System.Collections;
using System.Collections.Generic;
using Controllers.Input;
using UnityEngine;

namespace Controllers.CController {
    public class PlayerMovement : MonoBehaviour {
        private PlayerInputHandler input;
        private CharacterController controller;
        private Camera cam;

        public Vector3 Forward { get { return transform.forward; } }
        [SerializeField] private float movementSmoothstep = 5f;
        [SerializeField] private float movementSpeed = 20f;
        [SerializeField] private float jumpForce = 10;

        [Header("Ground")]
        [SerializeField] private float groundedGravity = 1;
        [SerializeField] private float gravity = 10;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundNormalDistance = 0.15f;

        private Vector3 horizontalSpeed = Vector3.zero;
        private float verticalSpeed;

        private bool canMove = true;

        public void Disable() {
            canMove = false;
        }

        public void Enable() {
            canMove = true;
        }

        private void Awake() {
            input = GetComponent<PlayerInputHandler>();
            controller = GetComponent<CharacterController>();
            cam = Camera.main;
        }

        void FixedUpdate() {
            // UpdateHorizontalSpeed();
            UpdateVerticalSpeed();

            Vector3 velocity = Vector3.zero;
            velocity += GetMovement();
            velocity += UpdateVerticalVector(velocity);
            // Turn player here
            controller.Move(velocity * Time.fixedDeltaTime);
        }

        Vector3 GetMovement() {
            if (!canMove) return Vector3.zero;

            Vector3 direction = ConvertInputWorldSpace(input.RawDirection);
            direction = Vector3.ProjectOnPlane(direction, GetGroundNormal()) * movementSpeed;
            // direction = Vector3.Lerp(horizontalSpeed, direction, movementSmoothstep * Time.fixedDeltaTime);
            return direction;
        }


        Vector3 UpdateVerticalVector(Vector3 velocity) {
            return velocity + Vector3.up * verticalSpeed;
        }

        void UpdateHorizontalSpeed() {
            Vector3 direction = ConvertInputWorldSpace(input.RawDirection);
            direction = Vector3.ProjectOnPlane(direction, GetGroundNormal()) * movementSpeed;
            horizontalSpeed = direction;
        }

        // Defacto jump
        void UpdateVerticalSpeed() {

            if (controller.isGrounded) {
                verticalSpeed = -groundedGravity;
                if (input.JumpPress && canMove) {
                    verticalSpeed = jumpForce;
                }
            }
            // Change this MoveTowards to Lerp so it follows the horizontal speed logic
            verticalSpeed = Mathf.MoveTowards(verticalSpeed, -gravity, Mathf.Pow(gravity, 2) * Time.deltaTime);
        }

        Vector3 GetGroundNormal() {
            if (Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, out RaycastHit hitInfo, groundNormalDistance + 0.015f, groundMask)) {
                return hitInfo.normal;
            }
            return Vector3.up; // vector down should never ever occur, so this value could show bugs 
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
