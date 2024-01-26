using System;
using UnityEngine;

[Serializable]
public class PlayerMove
{
    public enum PlayerStates
    {
        Idle,
        Walk,
        Run,
        Jump,
        CrouchIdle,
        CrouchWalk
    }

    [Header("States")]
    private PlayerStates playerStates;
    public PlayerStates GetPlayerStates => playerStates;

    [SerializeField] private CharacterController m_CharacterController;
    [SerializeField] private Animator m_CharacterAnimator;

    [Header("Speed")]
    [SerializeField] private float playerSpeed;
    [SerializeField] private float playerSpeedTime;
    [SerializeField] private float walkSpeed = 7.5f;
    [SerializeField] private float walkSpeedTime = 2f;
    [SerializeField] private float runSpeed = 11.5f;
    [SerializeField] private float runSpeedTime = 2f;
    [SerializeField] private float crouchSpeed = 3.5f;
    [SerializeField] private float crouchSpeedTime = 2f;
    [SerializeField] private float jumpSpeed = 8.0f;
    [SerializeField] private float jumpSpeedTime = 2f;
    [SerializeField] private float tireMultiple = 2f;
    [SerializeField] private float tireMultipleTime = 2f;

    [Header("Gravity")]
    [SerializeField] private float gravity = 10.0f;

    [Header("Move")]
    [SerializeField] public bool canMove = true;

    [Header("Ground")]
    [SerializeField] public bool isGrounded;
    [SerializeField] public bool sphereIsGrounded;

    [SerializeField] private float groundDistance = 0.55f;
    [SerializeField] private float groundRadius = 0.25f;

    private Vector3 moveDirection = Vector3.zero;
    private float movementDirectionY;

    [Header("Crouch")]
    [SerializeField] private bool isCrouch;
    [SerializeField] private bool crouchClamp;
    public bool getIsCrouch => isCrouch;

    [SerializeField] private float crouchCharacterHeight = 1.9f;
    //[SerializeField] private float crouchCharacterRadius = 0.6f;
    [SerializeField] private float crouchCharacterCenterY = -0.29f;

    [SerializeField] private float crouchUpDistance = 1.27f;
    [SerializeField] private float crouchUpRadius = 0.6f;

    [Header("Values")]
    [SerializeField] private float characterHeight;
    [SerializeField] private float characterRadius;
    [SerializeField] private float characterCenterY;
    
    [Header("Layer Mask")]
    [SerializeField] private LayerMask ignoreLayerMask;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float RunningFOV = 65.0f;
    [SerializeField] private float SpeedToFOV = 4.0f;
    [SerializeField] private float FOV;

    public PlayerDrawGizmos playerDrawGizmos;

    public bool forwardWall;
    public float moveDistance;

    //public Vector3 moveDirectionRay;
    public void Init(CharacterController characterController, Camera camera, Animator animator)
    {
        m_CharacterController = characterController;
        m_CharacterAnimator = animator;

        ignoreLayerMask = ~ignoreLayerMask;

        playerCamera = camera;

        characterHeight = m_CharacterController.height;
        characterRadius = m_CharacterController.radius;
        characterCenterY = m_CharacterController.center.y;
  
        FOV = playerCamera.fieldOfView;

        playerDrawGizmos = characterController.GetComponent<PlayerDrawGizmos>();
        playerDrawGizmos.ignoreLayerMask = ignoreLayerMask;
    }

    public void Move(Transform firstPersonController, PlayerInput playerInput)
    {
        PlayerControls(firstPersonController, playerInput);
        Gravity();
        SetupMove(firstPersonController, playerInput);
        OnJump(playerInput, movementDirectionY);
        GravityNULL();

        m_CharacterController.Move(moveDirection * Time.deltaTime);     
    }

    void SetupMove(Transform firstPersonController, PlayerInput playerInput)
    {
        Vector3 forward = firstPersonController.transform.TransformDirection(Vector3.forward);
        Vector3 right = firstPersonController.transform.TransformDirection(Vector3.right);

        playerSpeed = GetPlayerSpeed();

        float curSpeedY = canMove ? playerSpeed * playerInput.move.ReadValue<Vector2>().y : 0;
        float curSpeedX = canMove ? playerSpeed * playerInput.move.ReadValue<Vector2>().x : 0;

        movementDirectionY = moveDirection.y;
        //moveDirectionRay = curSpeedX > 0 || curSpeedY > 0 ? (forward * curSpeedY) + (right * curSpeedX) : moveDirectionRay;
        moveDirection = (forward * curSpeedY) + (right * curSpeedX);
    }

    void Gravity()
    {
        if (!isGrounded)
        {
            moveDirection.y -= gravity * Time.fixedDeltaTime;
        }
    }

    void GravityNULL()
    {
        if (isGrounded)
        {
            movementDirectionY = 0;
        }
    }

    void RunFOV(PlayerInput playerInput)
    {
        if (playerInput.run.IsPressed() && playerInput.move.ReadValue<Vector2>().y > 0)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, RunningFOV, SpeedToFOV * Time.deltaTime);
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, FOV, SpeedToFOV * Time.deltaTime);
        }
    }

    void OnJump(PlayerInput playerInput, float movementDirectionY)
    {
        bool Jump = playerInput.jump.IsPressed();

        if (m_CharacterAnimator.GetNextAnimatorStateInfo(0).IsName("Land"))
        {
            Jump = false;
        }

        if (Jump && isGrounded && !isCrouch)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
    }

    void OnCrouch()
    {
        if (m_CharacterAnimator.GetNextAnimatorStateInfo(0).IsName("Crouch"))
        {
            if (m_CharacterController.height >= crouchCharacterHeight - 0.01f)
            {
                m_CharacterController.height = Lerp(m_CharacterController.height, crouchCharacterHeight, 5);
                m_CharacterController.center = new Vector3(0, Lerp(m_CharacterController.center.y, crouchCharacterCenterY, 5), 0);
            }
        }
        if (m_CharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Crouch") && m_CharacterAnimator.GetNextAnimatorStateInfo(0).IsName("Locomotion"))
        {
            if (m_CharacterController.height <= characterHeight - 0.01f)
            {
                m_CharacterController.height = Lerp(m_CharacterController.height, characterHeight, 5);
                m_CharacterController.center = new Vector3(0, Lerp(m_CharacterController.center.y, characterCenterY, 5), 0);
            }
        }
    }

    void StayWall()
    {
        
        if (forwardWall)
        {
            canMove = false;

        }
        else
        {

            canMove = true;

        }
       
    }

    public void PlayerControls(Transform transform, PlayerInput playerInput)
    {
        StateMachine(playerInput);
        SetShpereIsGrounded(transform);
        CrouchControls(transform, playerInput);
        RunFOV(playerInput);
        //ForwardWall(transform);
        FallState();
        OnCrouch();
        SetIsGrounded();
        //StayWall();

        DrawGizmos();
    }

    float GetPlayerSpeed()
    {
        switch (GetPlayerStates)
        {
            case PlayerStates.Idle:
                return Lerp(playerSpeed, 0, playerSpeedTime);
            case PlayerStates.Walk:
                return Lerp(playerSpeed, walkSpeed, walkSpeedTime);        
            case PlayerStates.Run:
                return Lerp(playerSpeed, runSpeed, runSpeedTime);
            case PlayerStates.CrouchIdle:
                return Lerp(playerSpeed, 0, playerSpeedTime);
            case PlayerStates.CrouchWalk:
                return Lerp(playerSpeed, crouchSpeed, crouchSpeedTime);
            case PlayerStates.Jump:
                return Lerp(playerSpeed, jumpSpeed, jumpSpeedTime);
        }
        return Lerp(playerSpeed, 0, playerSpeedTime);
    }

    void StateMachine(PlayerInput playerInput)
    {
        if (isGrounded)
        {
            if (playerInput.move.ReadValue<Vector2>() == new Vector2(0, 0))
            {
                if (!isCrouch)
                {
                    playerStates = PlayerStates.Idle;
                }
                if (isCrouch)
                {
                    playerStates = PlayerStates.CrouchIdle;
                }
            }

            if (playerInput.move.ReadValue<Vector2>() != new Vector2(0, 0))
            {
                if (!isCrouch)
                {
                    if (playerInput.run.IsPressed() && playerInput.move.ReadValue<Vector2>().y > 0)
                    {
                        playerStates = PlayerStates.Run;
                    }
                    else
                    {
                        playerStates = PlayerStates.Walk;
                    }
                }
                if (isCrouch)
                {
                    playerStates = PlayerStates.CrouchWalk;
                }
            }
        }
        else
        {
            playerStates = PlayerStates.Jump;
        }
    }

    void SetIsGrounded()
    {
        isGrounded = m_CharacterController.isGrounded;
    }

    void SetShpereIsGrounded(Transform transform)
    {
        RaycastHit hit;
        Vector3 offset = new Vector3(0, 0.5f, 0);
        sphereIsGrounded = Physics.SphereCast(transform.position + offset, groundRadius, -transform.up, out hit, groundDistance, ignoreLayerMask);
    }

    void ForwardWall(Transform transform)
    {
        Vector3 positonRay = new Vector3(transform.position.x, playerCamera.transform.position.y, transform.position.z);
        Vector3 moveDir = new Vector3(moveDirection.x, 0, moveDirection.z);
        forwardWall = Physics.Raycast(positonRay, moveDir, moveDistance, ignoreLayerMask);     
    }

    void FallState()
    {
        if (!sphereIsGrounded && isGrounded)
        {

            m_CharacterController.radius = Lerp(m_CharacterController.radius, groundRadius, 25);
        }
        else
        {
            if (m_CharacterController.radius <= characterRadius - 0.01f)
            {
                m_CharacterController.radius = Lerp(m_CharacterController.radius, characterRadius, 25);
            }
        }
    }

    void CrouchControls(Transform transform, PlayerInput m_PlayerInput)
    {
        bool canStand;

        if (isCrouch)
        {
            RaycastHit hitu;
            canStand = !Physics.SphereCast(transform.position, crouchUpRadius, transform.up, out hitu, crouchUpDistance, ignoreLayerMask);
        }
        else
        {
            canStand = true;
        }

        if (crouchClamp)
        {
            if (canStand)
            {
                isCrouch = m_PlayerInput.crouch.IsPressed();
            }
        }
        else
        {
            if (isCrouch)
            {
                if (m_CharacterController.height != crouchCharacterHeight)
                {
                    if (m_PlayerInput.crouch.WasReleasedThisFrame() && canStand)
                    {
                        isCrouch = false;
                    }
                }
            }
            else
            {
                if (m_PlayerInput.crouch.WasReleasedThisFrame())
                {
                    isCrouch = true;
                }
            }
        }
    }

    float Lerp(float _old, float _new, float time)
    {
        return _old = Mathf.Lerp(_old, _new, time * Time.deltaTime);
    }

    void DrawGizmos()
    {
        playerDrawGizmos.crouchUpDistance = crouchUpDistance;
        playerDrawGizmos.crouchUpRadius = crouchUpRadius;
        playerDrawGizmos.groundDistance = groundDistance;
        playerDrawGizmos.groundRadius = groundRadius;
        playerDrawGizmos.moveDirection = moveDirection;
        playerDrawGizmos.playerCamera = playerCamera;
        playerDrawGizmos.moveDistance = moveDistance;
    }
}
