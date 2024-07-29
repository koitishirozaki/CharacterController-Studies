using UnityEngine;

namespace Controllers {
    public static class Utils {
 
        public static Vector3 ConvertInputWorldSpace(Camera cam, Vector2 input) {
            Vector3 worldSpace = new Vector3(input.x, 0, input.y).normalized;
            if (!cam) {
                Debug.LogError("Camera could not be found. Cant convert input to World Space, fallback to input space");
                return worldSpace;
            }
            worldSpace = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * worldSpace;
            return worldSpace;
        }

        public static Vector3 GetGroundNormal(Vector3 center, float range, LayerMask groundLayer) {
            if (Physics.Raycast(center + Vector3.up * 0.01f, Vector3.down, out RaycastHit hitInfo, range + 0.015f, groundLayer)) {
                return hitInfo.normal;
            }
            return Vector3.up; // vector down should never ever occur, so this value could show bugs 
        }
    }
}