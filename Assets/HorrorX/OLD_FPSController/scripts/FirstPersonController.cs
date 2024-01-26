using UnityEngine;
using Mirror;
using UnityEngine.Scripting.APIUpdating;
using System;
using System.Collections.Generic;
using System.Collections;


namespace PlayerController
{
    public class FirstPersonController : NetworkBehaviour
    {
        public PlayerInput playerInput;

        public MouseLook mouseLook;
        public Transform head;
        public Camera playerCamera;

        [Header("Move")]
        [SerializeField] public float moveSpeed = 5f;
        [Header("Jump")]
        [SerializeField] private float groundDrag = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpCooldown = 0.5f;
        [SerializeField] private float airMultiplier = 1f;
        [SerializeField] private float gravityScale;
        [Header("Ground")]
        [SerializeField] private bool isGrounded;
        [SerializeField] private float groundDistance = 0.55f;
        [SerializeField] private float groundRadius = 0.25f;
        [Header("Layers")]
        [SerializeField] private LayerMask ignoreLayerMask;

        private Transform orientation;
        private bool readyToJump;
        private float horizontalInput;
        private float verticalInput;
        private Vector3 moveDirection;

        private Rigidbody rb;

         [Header("Slope Handling")]
        public float maxSlopeAngle;
        private RaycastHit slopeHit;
        private bool exitingSlope;

        public bool isCrouch;

        public float playerSpeed;

        public enum PlayerStates
        {
            Idle,
            Walk,
            Run,
            Jump,
            CrouchIdle,
            CrouchWalk,
            Sliding
        }
        public PlayerStates playerStates;
        public float playerHeight;
        public float speedMultiplier;
        public float slopeMultiplier;
        public float lastplayerSpeed;
        
        
        public float runSpeed;
        public float walkSpeed;
        public float crouchSpeed;
        public float crouchUpRadius;
        public float crouchUpDistance;
        public bool crouchClamp;
        public CapsuleCollider capsuleCollider;
        public float crouchYScale;
        public float startYScale;

        public PlayerStates GetPlayerStates => playerStates;

        public void Awake()
        {
            ignoreLayerMask = ~ignoreLayerMask;
        }

        public void Start()
        {
            //orientation = new GameObject("Orientation").transform;
            //orientation.transform.parent = transform;
            //orientation.transform.position = new Vector3(0,0,0);
            playerInput.InputEnable();
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            mouseLook.Init(head,playerCamera.transform,playerInput);
        }

        public void Update()
        {       
            UpdateControl();         
        }
            
        public void FixedUpdate()
        {
            FixedUpdateControl();          
        }

        public void UpdateControl()
        {
            mouseLook.LookRotation(head, playerCamera.transform, playerInput);
            mouseLook.UpdateCursorLock(playerInput);
            GroundCheck();
            MoveModule();
            JumpModule();          
        }

        public void FixedUpdateControl()
        {
            MovePlayer();  
        }

        public void MoveModule()
        {
            horizontalInput = playerInput.move.ReadValue<Vector2>().x;
            verticalInput = playerInput.move.ReadValue<Vector2>().y;
            SpeedControl();
            RBDrag(); 
            StateMachine();
            CrouchControls();
        }

        public void JumpModule()
        {
            if (playerInput.jump.IsPressed() && readyToJump && isGrounded)
            {             
                OnJump();
                readyToJump = false;
            }
           
            if(isGrounded)
            {
                Invoke(nameof(ResetJump), jumpCooldown);
            }else{
                readyToJump = false;
                CancelInvoke(nameof(ResetJump));
            }
        }

        public void MovePlayer()
        {         
            moveDirection = head.forward * verticalInput + head.right * horizontalInput;

            /*
            if (OnSlope() && !exitingSlope)
            {
                rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

                if (rb.velocity.y > 0)
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            */

            if(isCrouch)
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);   
            }

            if (isGrounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

            else if (!isGrounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

            rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);

            //rb.useGravity = !OnSlope();
        }

        public void RBDrag()
        {
            if (isGrounded)
                rb.drag = groundDrag;
            else
                rb.drag = 0;   
        }

        private void SpeedControl()
        {        
            /*
            if (OnSlope() && !exitingSlope)
            {
                if (rb.velocity.magnitude > moveSpeed)
                    rb.velocity = rb.velocity.normalized * moveSpeed;
            }
            else
            {  */
                Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
               
                if (flatVel.magnitude > moveSpeed)
                {
                    Vector3 limitedVel = flatVel.normalized * moveSpeed;
                    rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                }
           // }           
        }

        private IEnumerator SmoothlyLerpMoveSpeed()
        {
            // smoothly lerp movementSpeed to desired value
            float time = 0;
            float difference = Mathf.Abs(playerSpeed - moveSpeed);
            float startValue = moveSpeed;

            while (time < difference)
            {
                moveSpeed = Mathf.Lerp(startValue, playerSpeed, time / difference);       
                time += Time.deltaTime * speedMultiplier;
                yield return null;
            }

            moveSpeed = playerSpeed;
        }

        public void OnJump()
        {       
            exitingSlope = true;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        public Vector3 GetSlopeMoveDirection(Vector3 direction)
        {
            return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
        }

        private void ResetJump()
        {
            readyToJump = true;

            exitingSlope = false;
        }


        void StateMachine()
        {
         
            if (isGrounded)
            {
                if (playerInput.move.ReadValue<Vector2>() == new Vector2(0, 0))
                {
                    if (!isCrouch)
                    {
                        playerStates = PlayerStates.Idle;
                        playerSpeed = walkSpeed;
                    }
                    if (isCrouch)
                    {
                        playerStates = PlayerStates.CrouchIdle;
                        playerSpeed = walkSpeed;
                    }
                }

                if (playerInput.move.ReadValue<Vector2>() != new Vector2(0, 0))
                {
                    if (!isCrouch)
                    {
                        if (playerInput.run.IsPressed() && playerInput.move.ReadValue<Vector2>().y > 0)
                        {
                            playerStates = PlayerStates.Run;
                            playerSpeed = runSpeed;
                        }
                        else
                        {
                            playerStates = PlayerStates.Walk;
                            playerSpeed = walkSpeed;
                        }
                    }
                    if (isCrouch)
                    {
                        playerStates = PlayerStates.CrouchWalk;
                        playerSpeed = crouchSpeed;
                    }
                }
            }
            else
            {
                playerStates = PlayerStates.Jump;
                playerSpeed = walkSpeed;
            }
            

            if(Mathf.Abs(playerSpeed - lastplayerSpeed) > 4f && moveSpeed != 0)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                moveSpeed = playerSpeed;
            }

            lastplayerSpeed = playerSpeed;
        }


        void CrouchControls()
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
                    isCrouch = playerInput.crouch.IsPressed();
                }
            }
            else
            {
                if (isCrouch)
                {
                    if (capsuleCollider.height != crouchYScale)
                    {
                        if (playerInput.crouch.WasReleasedThisFrame() && canStand)
                        {
                            isCrouch = false;
                        }
                    }
                }
                else
                {
                    if (playerInput.crouch.WasReleasedThisFrame())
                    {
                        isCrouch = true;
                    }
                }
            }
        }


        public void GroundCheck()
        {
            RaycastHit hit;
            Vector3 offset = new Vector3(0, 0.5f, 0);
            isGrounded = Physics.SphereCast(transform.position + offset, groundRadius, -transform.up, out hit, groundDistance, ignoreLayerMask);
        }

        private bool OnSlope()
        {
            if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }

            return false;
        }

        private Vector3 GetSlopeMoveDirection()
        {
            return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
        }

        public void OnDrawGizmos()
        {
            RaycastHit hitDown;
            Vector3 offset = new Vector3(0, 0.5f, 0);
            if (Physics.SphereCast(transform.position + offset, groundRadius, -transform.up, out hitDown, groundDistance, ignoreLayerMask))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position + offset, -transform.up * groundDistance);
                Gizmos.DrawWireSphere(transform.position + offset - transform.up * groundDistance, groundRadius);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position + offset, -transform.up * groundDistance);
                Gizmos.DrawWireSphere(transform.position + offset - transform.up * groundDistance, groundRadius);
            }

            RaycastHit hitUp;
            if (Physics.SphereCast(transform.position, crouchUpRadius, transform.up, out hitUp, crouchUpDistance, ignoreLayerMask))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, transform.up * crouchUpDistance);
                Gizmos.DrawWireSphere(transform.position + transform.up * crouchUpDistance, crouchUpRadius);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, transform.up * crouchUpDistance);
                Gizmos.DrawWireSphere(transform.position + transform.up * crouchUpDistance, crouchUpRadius);
            }
        }
    }
}