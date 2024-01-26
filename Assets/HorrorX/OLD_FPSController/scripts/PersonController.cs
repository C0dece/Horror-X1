using UnityEngine;
using Mirror;


namespace PlayerController
{
    public class PersonController : NetworkBehaviour
    {    
        public PlayerInput playerInput;

        [Header("Move")]
        [SerializeField] public float moveSpeed = 5f;
        [Header("Jump")]
        [SerializeField] private float groundDrag = 5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float jumpCooldown = 0.5f;
        [SerializeField] private float airMultiplier = 1f;
        [SerializeField] private float gravityScale;
        [Header("Ground")]
        [SerializeField] private bool IsGrounded;
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

        public void Awake()
        {
            ignoreLayerMask = ~ignoreLayerMask;
        }

        public void Start()
        {
            orientation = new GameObject("Orientation").transform;
            orientation.transform.parent = transform;
            orientation.transform.position = new Vector3(0,0,0);
            playerInput.InputEnable();
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
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
        }

        public void JumpModule()
        {
            if (playerInput.jump.IsPressed() && readyToJump && IsGrounded)
            {             
                OnJump();
                readyToJump = false;
            }
           
            if(IsGrounded)
            {
                Invoke(nameof(ResetJump), jumpCooldown);
            }else{
                readyToJump = false;
                CancelInvoke(nameof(ResetJump));
            }
        }

        public void MovePlayer()
        {         
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
                
            if (IsGrounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

            else if (!IsGrounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

            rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
        }

        public void RBDrag()
        {
            if (IsGrounded)
                rb.drag = groundDrag;
            else
                rb.drag = 0;   
        }

        public void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                    Vector3 limitedVel = flatVel.normalized * moveSpeed;
                    rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        public void OnJump()
        {              
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
        private void ResetJump()
        {
                readyToJump = true;
        }

        public void GroundCheck()
        {
            RaycastHit hit;
            Vector3 offset = new Vector3(0, 0.5f, 0);
            IsGrounded = Physics.SphereCast(transform.position + offset, groundRadius, -transform.up, out hit, groundDistance, ignoreLayerMask);
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
        }
    }
}