using Cinemachine;
using RBCC.Scripts.Utils;
using UnityEngine;

namespace RBCC.Scripts.Cameras
{
    public class FirstPersonCamera : MonoBehaviour
    {
        public float sensitivity = 1f;

        private float _currentSensitivity; // Used to update sensitivity at runtime.

        private void OnEnable()
        {
            SetCameraSensitivity(sensitivity);
        }

        void Update()
        {
            if (!MathUtils.EqualApprox(sensitivity, _currentSensitivity))
            {
                SetCameraSensitivity(sensitivity);
            }
        }

        private void SetCameraSensitivity(float value)
        {
            CinemachineVirtualCamera vCam = GetComponent<CinemachineVirtualCamera>();
            vCam.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = value;
            vCam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = value;
            _currentSensitivity = value;
        }
    }
}
