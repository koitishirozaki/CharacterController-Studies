using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers.SimpleMovement {
    public class FollowGround : MonoBehaviour {
        public Transform transformToSnap;
        public LayerMask ground;

        void Update() {
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 1000, ground)) {
                transformToSnap.position = hit.point;
            }
        }
    }
}
