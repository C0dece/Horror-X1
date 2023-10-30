using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RBCC.Scripts.Cameras
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class ThirdPersonCamera : MonoBehaviour
    {
        public Transform followTransform;
        public float lookSensitivity = 1.5f;

        [SerializeField] [Range(10f, 80f)] private float maxAngle = 40f;
        [SerializeField] [Range(-80f, -10f)] private float minAngle = -20f;
    
        public bool invertXAxis;
        public bool invertYAxis;

        private Transform _follow;
        private Vector2 _look;

        public void OnLook(InputAction.CallbackContext context)
        {
            _look = context.ReadValue<Vector2>();
            if (invertXAxis) _look.x = -_look.x;
            if (invertYAxis) _look.y = -_look.y;
        }

        private void Start()
        {
            if (followTransform == null)
            {
                Debug.Log("No target set on 3rd Person Camera. Try to find character.");
                followTransform = GameObject.FindWithTag("Player").transform;
            }
        
            GameObject runtimeTarget = new GameObject("ThirdPersonTargetRuntime");
            _follow = runtimeTarget.transform;
            CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();
            vcam.Follow = _follow;
            vcam.LookAt = _follow;
            _follow.position = followTransform.position;
            _follow.rotation = Quaternion.identity;
        }

        private void Update()
        {
            // Update target position to match with the real one
            _follow.position = followTransform.position;
        
            #region Follow Transform Rotation

            //Rotate the Follow Target transform based on the input
            _follow.transform.rotation *= Quaternion.AngleAxis(_look.x * lookSensitivity, Vector3.up);

            #endregion

            #region Vertical Rotation
            _follow.transform.rotation *= Quaternion.AngleAxis(_look.y * lookSensitivity, Vector3.right);

            // Matrix4x4 transformMatrix = Matrix4x4.TRS(
            //     _follow.position,
            //     Quaternion.LookRotation(Vector3.Cross(debugX, debugY), debugY),
            //     Vector3.one);

            var angles = _follow.transform.localEulerAngles;
            angles.z = 0f;

            var angle = angles.x;

            //Clamp the Up/Down rotation
            if (angle > 180 && angle < 360 + minAngle)
            {
                angles.x = 360 + minAngle;
            }
            else if(angle < 180 && angle > maxAngle)
            {
                angles.x = maxAngle;
            }
        
            _follow.transform.localEulerAngles = angles;
            #endregion
        }
    }
}
