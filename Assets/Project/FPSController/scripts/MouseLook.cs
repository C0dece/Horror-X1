using UnityEngine;
using Mirror;
using UnityEngine.Scripting.APIUpdating;
using System;
using UnityEngine.InputSystem;


namespace PlayerController
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity;
        public float YSensitivity;
         public float xRot;
        public float yRot;
        public float rotLimit = 30f;
        public Quaternion m_CameraTargetRot;
         public bool smooth;
        public float smoothTime = 5f;
        public Transform orientation;
        public bool lockCursor = true;
        public bool lockCamera = false;
        private bool m_cursorIsLocked = true;
        public void Init(Transform head, Transform camera, PlayerInput input)
        {
            //input.InputEnable();
        }

        public void LookRotation(Transform head, Transform camera, PlayerInput input)
        {
            camera.position = head.transform.position;

            float MouseX = input.mouseX.ReadValue<float>() * XSensitivity * Time.deltaTime;
            float MouseY = input.mouseY.ReadValue<float>() * YSensitivity * Time.deltaTime;

            if(MouseX >= rotLimit)
            {
                MouseX = rotLimit;
            }
            if(MouseX <=-rotLimit)
            {
                MouseX = -rotLimit;
            }

            yRot += MouseX;
            if(yRot >= 360)
            {
                yRot = MouseX;
            }
            if(yRot <= 0)
            {
                yRot = 360;
            }

            xRot -= MouseY;
            xRot = Mathf.Clamp(xRot, -90f, 90f);

            if (smooth)
            {
                head.rotation = Quaternion.Slerp(head.rotation, Quaternion.Euler(0, yRot, 0),
                    smoothTime * Time.deltaTime);
                camera.rotation = Quaternion.Slerp(camera.rotation, Quaternion.Euler(xRot, yRot, 0),
                    smoothTime * Time.deltaTime);
            }
            else
            {
                camera.rotation = Quaternion.Euler(xRot, yRot, 0);
                head.rotation = Quaternion.Euler(0, yRot, 0);
            }
        }

        public void UpdateCursorLock(PlayerInput input)
        {      
            if (lockCursor)
                InternalLockUpdate(input);
        }

        private void InternalLockUpdate(PlayerInput input)
        {
            if (input.escape.IsPressed())
            {
                m_cursorIsLocked = false;
            }
            else if (input.leftMouseButton.IsPressed())
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}