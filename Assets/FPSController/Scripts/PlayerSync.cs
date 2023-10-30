using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using oldfpc;
namespace oldfpc{

public class PlayerSync : NetworkBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField, SyncVar] private Quaternion cameraRotation;

    void Start()
    {
        mainCamera = GetComponent<FirstPersonController>().playerCamera;
    }

    void Update()
    {
        
    }

    private void LateUpdate()
    {
        CameraPosition();
        SetCameraPositionY();
    }
    public void CameraPosition()
    {
        if (!isLocalPlayer)
        {
            mainCamera.transform.localRotation = Quaternion.Lerp(mainCamera.transform.localRotation, cameraRotation, 5f *  Time.deltaTime);
        }
    }

    [Client]
    public void SetCameraPositionY()
    {
        if (isLocalPlayer)
        {
            CmdSetCameraPositionY(mainCamera.transform.rotation);
        }
    }
    [Command]
    public void CmdSetCameraPositionY(Quaternion rot)
    {
        cameraRotation = rot;
    }
}
}