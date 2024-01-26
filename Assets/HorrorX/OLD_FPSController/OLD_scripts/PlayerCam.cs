using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float XSensitivity;
    public float YSensitivity;

    public Transform head;

    public Transform orientation;

    public float xRot;
    public float yRot;

    public PlayerInput input;
    // Start is called before the first frame update
    void Start()
    {
        input.Setup();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = head.transform.position;

        float MouseX = input.mouseX.ReadValue<float>() * XSensitivity;
        float MouseY = input.mouseY.ReadValue<float>() * YSensitivity;

        yRot += MouseX;

        xRot -= MouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.rotation = Quaternion.Euler(0, yRot, 0);
    }
}
