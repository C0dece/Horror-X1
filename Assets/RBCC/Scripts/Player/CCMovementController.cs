using UnityEngine;
using UnityEngine.InputSystem;

namespace RBCC.Scripts.Player
{
    public class CCMovementController : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;
        [SerializeField] private float speed;

        private float _horizontal;
        private float _vertical;

        private void FixedUpdate()
        {
            Vector3 moveDir = new Vector3(_horizontal, 0f, _vertical);

            Vector3 gravityForce = Physics.gravity * Time.fixedDeltaTime;

            Vector3 movement = speed * Time.fixedDeltaTime * moveDir + gravityForce;
        
            controller.Move(movement);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _horizontal = context.ReadValue<Vector2>().x;
            _vertical = context.ReadValue<Vector2>().y;
        }
    }
}
