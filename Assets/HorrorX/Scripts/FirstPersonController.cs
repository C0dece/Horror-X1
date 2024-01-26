using UnityEngine;
using Mirror;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.InputSystem;


namespace oldfpc{
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : NetworkBehaviour
{

    public CharacterController characterController;
  
    public Camera playerCamera;

    public MouseLook mouseLook;
    public PlayerMove playerMove;
    public PlayerAnimator playerAnimator;

    public Animator animator;
    public NetworkAnimator netAnimator;
 
    public SkinnedMeshRenderer headMesh;

    public PlayerInput playerInput;


    void Awake()
    {
        animator = GetComponent<Animator>();
        netAnimator = GetComponent<NetworkAnimator>();
        characterController = GetComponent<CharacterController>();
    }

    public void Start()
    {
        playerInput.Setup();

        mouseLook.Init(transform, playerCamera.transform, playerInput);
        playerMove.Init(characterController, playerCamera, animator);
        playerAnimator.Init(animator, netAnimator);

        if (!isLocalPlayer)
        {         
            headMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
        else
        {
            playerCamera.depth = 1;
            headMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }

   

    void Update()
    {


        playerAnimator.SetCameraPosition(mouseLook,playerCamera.transform);

        if (!isLocalPlayer)
        {         
            return;
        }


        Move();
        Animate(); 
    }

   
    private void LateUpdate()
    {     

        if (!isLocalPlayer)
        {
            return;
        }
      
        Rotate();
    }

   

    void Move()
    {
        playerMove.Move(transform, playerInput);
    }
    void Rotate()
    {
        mouseLook.LookRotation(transform, playerCamera.transform, playerInput);
        mouseLook.UpdateCursorLock(playerInput);
    }
    void Animate()
    {
        playerAnimator.Animate(mouseLook,playerMove, playerInput);
    } 
}
}