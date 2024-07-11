using System.Collections;
using System.Collections.Generic;
using Controllers.Input;
using UnityEngine;
using MyBox;

namespace Controllers.SimpleMovement {
    /*
        Uses rigidbody.velocity to alter its movement
        The important thing in this controller is how you setup the gameobject and the position.
        The main pivot (roots) should be the one close to the ground
    */
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerController : MonoBehaviour {
        PlayerInputHandler inputHandler;
        Rigidbody rb;
        Camera cam;

        [Header("Movement")]
        [SerializeField] private float movementSpeed = 40;
        [SerializeField] private float movementSmoothstep = 500;

        [Space(10), Header("Gravity")]
        [SerializeField] private float gravityForce = 10f;

        [Space(10), Header("Jump")]
        // [SerializeField, ReadOnly] private bool jumpPressed = false;
        // [SerializeField] private float jumpForce = 2f;
        // [SerializeField] private float fallMultiplier = 2.5f;

        [Space(10), Header("Ground Detection")]
        [SerializeField, ReadOnly] private bool IsGrounded;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Vector3 groundCheckerScale = Vector3.one;
        [SerializeField] private float groundNormalDistance = 0.1f;

        void Start() {
            inputHandler = GetComponent<PlayerInputHandler>();
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            cam = Camera.main;
        }

        void Update() {
            // jumpPressed = inputHandler.JumpPress;
        }

        void FixedUpdate() {
            CheckGround();

            Vector3 velocity = Vector3.zero;

            velocity += GetMovement(ConvertInputWorldSpace(inputHandler.RawDirection));
            velocity += GetGravity(gravityForce);
            // velocity += GetJumpVector(jumpForce);

            rb.velocity = velocity;
        }

        Vector3 GetMovement(Vector3 input) {
            Vector3 movement = Vector3.zero;
            movement = input;
            movement *= movementSpeed;
            movement = Vector3.ProjectOnPlane(movement, GetGroundNormal());
            movement = Vector3.Lerp(rb.velocity, movement, movementSmoothstep * Time.fixedDeltaTime);
            Debug.DrawRay(transform.position, movement, Color.green);
            return movement;
        }
        Vector3 GetGravity(float force) {
            if (IsGrounded) return Vector3.zero;
            Vector3 gravity = Vector3.down * Mathf.Pow(gravityForce, 2) * Time.fixedDeltaTime;
            return gravity;
        }
        // ! Let's not use the Jump option until we figure out a way to improve this lmao 
        Vector3 GetJumpVector(float force) {
            Vector3 jump = Vector3.zero;
            // if (IsGrounded && jumpPressed) {
            //     jump = rb.velocity + Vector3.up * gravityForce * (jumpForce - 1) * Time.fixedDeltaTime;
            // }
            return jump;
        }

        void CheckGround() {
            Collider[] colliders = Physics.OverlapBox(transform.position, groundCheckerScale, transform.rotation, groundMask);
            if (colliders.Length > 0) IsGrounded = true;
            else IsGrounded = false;
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
        void OnDrawGizmos() {
            // Ground Checking
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, groundCheckerScale);

            // Ground Normal
            if (IsGrounded) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundNormalDistance);
            }
        }
    }
}
