using System.Collections;
using System.Collections.Generic;
using Controllers.Input;
using UnityEngine;
using MyBox;

namespace Controllers.SimpleMovement {
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
        [SerializeField] private float gravityAcceleration = 1;
        [SerializeField, ReadOnly] private Vector3 gravityVector = Vector3.zero;

        [Space(10), Header("Jump")]
        [SerializeField] private float jumpForce = 2;
        [SerializeField] private float jumpRateChange = 1;
        [SerializeField, ReadOnly] private bool isJumping = false;
        [SerializeField, ReadOnly] private bool canJump = true;
        [SerializeField, ReadOnly] Vector3 jumpVector = Vector3.zero;

        [Space(10), Header("Ground Detection")]
        [SerializeField, ReadOnly] private bool IsGrounded;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Vector3 groundCheckerScale = Vector3.one;
        [SerializeField] private float groundNormalDistance = 0.1f;

        [Header("Debug")]
        [SerializeField, ReadOnly] private Vector3 velocity = Vector3.zero;

        void Start() {
            inputHandler = GetComponent<PlayerInputHandler>();
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            cam = Camera.main;
        }

        void Update() {
            if (!inputHandler.JumpPress) canJump = true;

            if (inputHandler.JumpPress && IsGrounded && canJump) {
                Jump();
            }
        }

        void FixedUpdate() {
            CheckGround();

            velocity = Vector3.zero;

            velocity += GetMovement(Utils.ConvertInputWorldSpace(cam, inputHandler.RawDirection));
            velocity += GetGravity(gravityForce);
            velocity += jumpVector;

            rb.velocity = velocity;

            UpdateJumpVector();
        }

        Vector3 GetMovement(Vector3 worldInput) {
            Vector3 movement = Vector3.zero;
            movement = worldInput;
            movement *= movementSpeed;
            movement = Vector3.ProjectOnPlane(movement, Utils.GetGroundNormal(rb.position, groundNormalDistance, groundMask));
            movement = Vector3.Lerp(rb.velocity, movement, movementSmoothstep * Time.fixedDeltaTime);
            Debug.DrawRay(transform.position, movement, Color.green);
            return movement;
        }
        Vector3 GetGravity(float force) {
            if (IsGrounded) {
                gravityVector = Vector3.zero;
                return Vector3.zero;
            }
            gravityVector = Vector3.down * Mathf.MoveTowards(gravityVector.magnitude, gravityForce, gravityAcceleration);
            return gravityVector;
        }

        void UpdateJumpVector() {
            jumpVector = Vector3.up * Mathf.MoveTowards(jumpVector.magnitude, 0, jumpRateChange);
        }

        void Jump() {
            jumpVector = jumpForce * Vector3.up;
            isJumping = true;
            canJump = false;
        }

        void CheckGround() {
            Collider[] colliders = Physics.OverlapBox(transform.position, groundCheckerScale, transform.rotation, groundMask);
            if (colliders.Length > 0) {
                isJumping = false;
                IsGrounded = true;
            } else IsGrounded = false;
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, groundCheckerScale);

            if (IsGrounded) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundNormalDistance);
            }
        }
    }
}
