
using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    public bool sphereIsGrounded;
    [SerializeField] private float groundDistance = 0.55f;
    [SerializeField] private float groundRadius = 0.25f;

    [SerializeField] private LayerMask ignoreLayerMask;

    Rigidbody rb;

    public float gravityScale;

    

    private void Awake()
    {
        ignoreLayerMask = ~ignoreLayerMask;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        SetShpereIsGrounded(transform);

        MyInput();
        SpeedControl();

        // handle drag
        if (sphereIsGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }
    
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && sphereIsGrounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (sphereIsGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!sphereIsGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    void SetShpereIsGrounded(Transform transform)
    {
        RaycastHit hit;
        Vector3 offset = new Vector3(0, 0.5f, 0);
        sphereIsGrounded = Physics.SphereCast(transform.position + offset, groundRadius, -transform.up, out hit, groundDistance, ignoreLayerMask);
    }


    void OnDrawGizmos()
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
