using UnityEngine;

namespace Controllers.ModularRigidbody {
    public interface IPlayerMovement {
        void SetInput(Vector2 inputDirection);
    }
}