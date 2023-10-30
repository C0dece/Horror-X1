using UnityEngine;
using UnityEngine.InputSystem;

namespace RBCC.Scripts.Examples
{
    public class CameraSwitcher : MonoBehaviour
    {
        public GameObject[] cameras;

        public InputAction switchCameraAction;

        private int _currentCameraIdx;

        private void OnEnable()
        {
            switchCameraAction.performed += OnSwitchCamera;
            switchCameraAction.Enable();

            foreach (GameObject camera in cameras)
            {
                camera.SetActive(false);
            }
            cameras[0].SetActive(true);
        }

        private void OnDisable()
        {
            switchCameraAction.performed -= OnSwitchCamera;
        }

        public void OnSwitchCamera(InputAction.CallbackContext context)
        {
            cameras[_currentCameraIdx].SetActive(false);
        
            _currentCameraIdx = (_currentCameraIdx + 1) % cameras.Length;
        
            cameras[_currentCameraIdx].SetActive(true);
        }
    }
}
