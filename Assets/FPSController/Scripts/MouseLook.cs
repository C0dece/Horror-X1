using Mirror;
using System;
using UnityEngine;


namespace oldfpc
{
[Serializable]
public class MouseLook
{
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;

    public bool clampVerticalRotation = true;

    public float MinimumX = -90F;
    public float MaximumX = 90F;

    public bool smooth;
    public float smoothTime = 5f;

    public bool lockCursor = true;
    public bool lockCamera = false;

    public Vector3 FPS_CameraOffset;

    public Quaternion m_CharacterTargetRot;
    public Quaternion m_CameraTargetRot;

    private bool m_cursorIsLocked = true;

    //private PlayerInput m_PlayerInput;

    public float _yRot;
    public float _yRotClampAngle = 10f;

    float xRot;
    float yRot;

    public void Init(Transform character, Transform camera, PlayerInput input)
    {
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
        //m_PlayerInput = input;
    }

    public void Init(Transform character, Transform camera)
    {
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
        //m_PlayerInput = input;
    }

    public void LookRotation(Transform character, Transform camera, PlayerInput input)
    {
        //UpdateCursorLock();
        if (!lockCamera)
        {
            
            float yRot = input.mouseX.ReadValue<float>() * XSensitivity;
            float xRot = input.mouseY.ReadValue<float>() * YSensitivity;

            _yRot = input.mouseX.ReadValue<float>();


            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }
        }
    }

    public void LookRotation(Transform character, Transform camera)
    {
        //UpdateCursorLock();
        if (!lockCamera)
        {

            float yRot = Input.GetAxisRaw("Mouse X") * Time.deltaTime;//input.mouseX.ReadValue<float>() * XSensitivity;
            float xRot = Input.GetAxisRaw("Mouse Y") * Time.deltaTime;//input.mouseY.ReadValue<float>() * YSensitivity;

            _yRot = Input.GetAxisRaw("Mouse X") * Time.deltaTime;//input.mouseX.ReadValue<float>();


            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }
        }
    }

    /*
    public void LookRotationInput() 
    {
        yRot = m_PlayerController.Player.MouseY.ReadValue<float>() * XSensitivity;
        xRot = m_PlayerController.Player.MouseX.ReadValue<float>() * YSensitivity;
        _yRot = m_PlayerController.Player.MouseY.ReadValue<float>();
    }
    */
    public void UpdateCursorLock(PlayerInput input)
    {      
        if (lockCursor)
            InternalLockUpdate(input);
    }

    public void UpdateCursorLock()
    {
        if (lockCursor)
            InternalLockUpdate();
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

    private void InternalLockUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButton(0))
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

    public Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

}
}
