using RBCC.Scripts.Environment.Platforms;
using RBCC.Scripts.Player.PlayerStates;
using RBCC.Scripts.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RBCC.Scripts.Player
{
    public class PlayerStateController : MonoBehaviour
    {
        // State Machine
        [SerializeField] private PlayerStateFactory stateFactory;
        private PlayerBaseState _currentRootState;

        [Header("Components")]
        [Tooltip("Camera of the player. Main Camera if set to None.")]
        public Transform cam;
        private Rigidbody _rb;

        [Header("Ground Ray")]
        public LayerMask groundLayer;
        [Tooltip("Ground ray distance to detect ground and apply floating forces.")]
        public float groundRayLength = 3f;
        public Vector3 groundRayDir = Physics.gravity.normalized;
        
        private bool _groundDetected;
        private RaycastHit _groundHit;
        private float _groundCoyoteTime; // Time since the last ground detected
        private Vector3 _currentGravity;
        
        [Header("Wall Ray")]
        public LayerMask wallLayer;
        [Tooltip("Range to detect walls.")]
        public float wallRayLength = 3f;
        public Vector3 wallRayLocalDir = Vector3.forward;

        private bool _wallDetected;
        private RaycastHit _wallHit;
        
        [Header("Coyote delays")]
        [Tooltip("Time allowed after leaving the ground to jump.")]
        public float jumpCoyoteDelay = 0.1f;
        [Tooltip("Number of jumps allowed. Default 1. If you want double jump set it to 2 etc.")]
        public int maxJumps = 1;

        [Header("Rotation")] 
        [Tooltip("Torque values to maintain the character vertical.")]
        [SerializeField] private float jointSpringStrength = 100f;
        [SerializeField] private float jointSpringDamper = 10f;
        [Tooltip("Ratio to rotate the transform to smoothly rotate the character in the moving direction.")]
        [SerializeField] private float turnSmoothRatio = 0.05f;

        [Header("Current Horizontal Movement")]
        [Tooltip("Max slope angle before slipping.")]
        public float maxSlopeAngle = 20f;
        public float maxSpeed = 8f;
        public float acceleration = 50f;
        public float maxAccelerationForce = 150f;
        public float deceleration = 50f;
        public float maxDecelerationForce = 150f;
        public AnimationCurve accelerationFactorFromDot;
        public AnimationCurve maxAccelerationForceFactorFromDot;

        // if the player is trying to move to a specific direction. Decides whether apply acceleration or deceleration.
        private bool _isAccelerating; 
        private Vector3 _targetVelocity = Vector3.zero;
        private Vector3 _externalVelocity;
        
        [Header("Movement Multipliers")]
        public float maxSpeedMultiplier = 1f;
        public float maxAccelerationMultiplier = 1f;
        
        // Inputs
        private const float InputOffset = 0.3f; // Input magnitude necessary to consider movement inputs. (useful for offset with joystick on gamepad for ex)
        private bool _horizontalInputsDetected; // If player inputs are detected for moving
        private float _horizontal;
        private float _vertical;
        private Vector3 _inputDirection; // Raw input direction
        private Vector3 _movingDirection; // Direction the player is moving (=_headingDirection unless external thing modify it)
        private Vector3 _movingDirectionNormal; // Normal dir used to limit the input direction to a specific plan
        private Vector3 _headingDirection; // Direction the player is trying to move
        private bool _isSliding;
        private bool _isJumpingUp;

        private bool _runKeyPerformed;
        private bool _crouchKeyPerformed;
        private bool _jumpKeyPerformed;

        // Miscellaneous
        // Current number of jumps in the air.
        private int _jumps = 0;
        // Is the character currently allowed to jump.
        private bool _allowJump = true;

        #region Events

        // Input events
        // These events are invoked from the PlayerInput component.
        // You can register delegates (functions) to it in PlayerStates for instance. (See CrouchState for example).
        public delegate void InputFired(InputAction.CallbackContext context);
        public event InputFired OnJumpPressed;
        public event InputFired OnRunPressed;
        public event InputFired OnCrouchPressed;
        public event InputFired OnDashPressed;
        
        // Custom events
        // Events invoked when landing, taking off and hitting a wall.
        // As input events, you can register delegates to be executed when these events are fired.
        public delegate void PlayerMovementChange();
        public event PlayerMovementChange OnLanding;
        public event PlayerMovementChange OnTakeOff;
        public event PlayerMovementChange OnWallHit;

        // State delegates.
        // Invoked whenever the player changes state.
        public delegate void PlayerStateChange(PlayerState newState);
        public PlayerStateChange OnEnterState;
        public PlayerStateChange OnExitState;

        #endregion

        #region Getters and Setters

        public PlayerBaseState CurrentRootState
        {
            get => _currentRootState;
            set => _currentRootState = value;
        }
        
        public Rigidbody Rb => _rb;

        /// <summary>
        /// Current gravity applied to the character (0 when grounded).
        /// </summary>
        public Vector3 CurrentGravity => _currentGravity;
        
        public float JointSpringStrength
        {
            get => jointSpringStrength;
            set => jointSpringStrength = value;
        }

        public float JointSpringDamper
        {
            get => jointSpringDamper;
            set => jointSpringDamper = value;
        }

        /// <summary>
        /// Ratio added to the joint spring to rotate faster the character.
        /// </summary>
        public float TurnSmoothRatio
        {
            get => turnSmoothRatio;
            set => turnSmoothRatio = value;
        }
        
        /// <summary>
        /// Time elapsed since the character was lastly grounded.
        /// </summary>
        public float GroundCoyoteTime => _groundCoyoteTime;

        /// <summary>
        /// True if horizontal inputs are detected (magnitude of XY axis > 1). Usually if the player is pressing WASD.
        /// </summary>
        public bool HorizontalInputsDetected
        {
            get => _horizontalInputsDetected;
        }

        /// <summary>
        /// UpDirection of this character.
        /// </summary>
        public Vector3 UpDirection => -groundRayDir;

        /// <summary>
        /// Hit value of the ground below the character if grounded (layer detection). If not, this is the last hit value.
        /// </summary>
        public RaycastHit GroundHit => _groundHit;
        
        /// <summary>
        /// Hit value or last Hit value of the wall in front of the character.
        /// </summary>
        public RaycastHit WallHit => _wallHit;

        /// <summary>
        /// If a wall in front of the character is detected (layer detection). 
        /// </summary>
        public bool WallDetected => _wallDetected;

        /// <summary>
        /// Current movement force direction applied to this character.
        /// </summary>
        public Vector3 MovingDirection => _movingDirection;
        
        /// <summary>
        /// Normalized raw horizontal input direction.
        /// </summary>
        public Vector3 InputDirection => _inputDirection;

        /// <summary>
        /// Direction the character is trying to move to.
        /// Different than moving direction as some external forces can modify the moving direction.
        /// </summary>
        public Vector3 HeadingDirection => _headingDirection;
        
        /// <summary>
        /// Horizontal direction the player is looking at with the camera (projected on the XZ plane).
        /// </summary>
        public Vector3 LookingDirection => Quaternion.Euler(0f, cam.eulerAngles.y, 0f) * Vector3.forward;

        public Vector3 CurrentVelocity => _rb.velocity;

        public bool IsGrounded => _groundDetected;

        public bool IsSliding => _isSliding;

        /// <summary>
        /// Is the character is going up while in air after jumping.
        /// </summary>
        public bool IsJumpingUp
        {
            get => _isJumpingUp;
            set => _isJumpingUp = value;
        }

        /// <summary>
        /// Current number of jumps. (Useful for double jump for instance).
        /// </summary>
        public int Jumps
        {
            get => _jumps;
            set => _jumps = value;
        }

        /// <summary>
        /// Can the character jump ? (for ex: not in crouch state)
        /// If you want to permanently disable jump, disable the jump state instead.
        /// </summary>
        public bool AllowJump
        {
            get => _allowJump;
            set => _allowJump = value;
        }

        // Inputs
        
        /// <summary>
        /// Is the Run key currently performed.
        /// </summary>
        public bool RunKeyPerformed => _runKeyPerformed;
        
        /// <summary>
        /// Is the Crouch key currently performed.
        /// </summary>
        public bool CrouchKeyPerformed => _crouchKeyPerformed;
        
        /// <summary>
        /// Is the Jump key currently performed.
        /// </summary>
        public bool JumpKeyPerformed => _jumpKeyPerformed;

        #endregion

        #region Gizmos

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Ground ray
            Vector3 startPosition = transform.position;
            Vector3 endPosition = startPosition + groundRayLength * groundRayDir;
            GizmosUtils.DrawThickLine(
                startPosition,
                endPosition,
                5f,
                Color.white);
            
            // Wall ray
            // Vector3 wallDir = MathUtils.EqualApprox(_headingDirection.magnitude, 0f)
            //     ? Vector3.ProjectOnPlane(transform.forward, UpDirection)
            //     : _headingDirection;
            // startPosition = transform.position;
            // endPosition = startPosition + wallRayLength * wallDir;
            // GizmosUtils.DrawThickLine(
            //     startPosition,
            //     endPosition,
            //     5f,
            //     Color.gray);

            if (_groundDetected)
            {
                // Ground normal
                // startPosition = _groundHit.point;
                // endPosition = startPosition + groundRayLength * _groundHit.normal;
                // GizmosUtils.DrawThickLine(
                //     startPosition,
                //     endPosition,
                //     5f,
                //     Color.black);
            
                // Slope direction
                // float slopeAngle = Vector3.Angle(rayDir, -_groundHit.normal);
                // Vector3 normal = Vector3.Cross(-_groundHit.normal, rayDir);
                // Vector3 forceDir = Quaternion.AngleAxis(90f + slopeAngle, normal) * -_groundHit.normal;
                // startPosition = transform.position;
                // endPosition = startPosition + groundRayLength * forceDir;
                // GizmosUtils.DrawThickLine(
                //     startPosition,
                //     endPosition,
                //     5f,
                //     Color.black);
            
            }
        }
#endif

        #endregion
    
        #region Initialization

        private void OnEnable()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Awake()
        {
            // Setup states
            stateFactory.SetContext(this);
            _currentRootState = stateFactory.Ground();
            _currentRootState.EnterStates();
        }

        private void Start()
        {
            // Affect the main Camera as the current camera by default.
            // Change this if you want to use a custom camera.
            if (cam == null)
                cam = Camera.main.transform;
        
            // Normalize it as it should be a direction
            groundRayDir.Normalize();
        }

        #endregion

        #region Update

        private void Update()
        {
            _currentRootState.UpdateStates();
        
            // Handle the smooth rotation (torque in fixed update).
            HandleRotation();
        }

        #endregion

        #region Fixed update

        private void FixedUpdate()
        {
            // Update states
            _currentRootState.FixedUpdateStates();
        
            DetectGround();
            DetectSlope();
            DetectWall();
        
            HandleInputDirection();
            HandlePlayerGravity();
            HandleTorqueRotation();

            if (_movingDirectionNormal.magnitude > 0f) 
                _movingDirection = Vector3.ProjectOnPlane(_movingDirection, _movingDirectionNormal.normalized);
            // Debug.Log(_movingDirectionNormal);

            HandleMovementMultipliers();
            HandleHorizontalMovement();
        }

        #endregion
    
        #region Inputs

        // Triggered in the PlayerInput component on the player.
        
        public void OnMove(InputAction.CallbackContext context)
        {
            _horizontal = context.ReadValue<Vector2>().x;
            _vertical = context.ReadValue<Vector2>().y;
        }
    
        public void OnRun(InputAction.CallbackContext context)
        {
            _runKeyPerformed = context.performed;
            OnRunPressed?.Invoke(context);
        }
        
        public void OnCrouch(InputAction.CallbackContext context)
        {
            _crouchKeyPerformed = context.performed;
            OnCrouchPressed?.Invoke(context);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            _jumpKeyPerformed = context.performed;
            OnJumpPressed?.Invoke(context);
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            OnDashPressed?.Invoke(context);
        }

        #endregion

        #region Fixed Updates Handles
    
        /// <summary>
        /// Adjust the _inputDirection with the camera angle and check enabled inputs.
        /// </summary>
        private void HandleInputDirection()
        {
            _inputDirection = new Vector3(_horizontal, 0f, _vertical);
            _inputDirection.Normalize();
    
            // Detect horizontal player inputs
            _horizontalInputsDetected = (_inputDirection.magnitude >= InputOffset);
    
            // Set moving direction
            _movingDirection = _inputDirection;

            // Adapt player movement with camera
            if (_horizontalInputsDetected)
            {
                // Rotate input direction so that forward is the camera forward
                var targetAngle = Mathf.Atan2(_movingDirection.x, _movingDirection.z) * Mathf.Rad2Deg
                                  + cam.eulerAngles.y;
                _movingDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _movingDirection.Normalize();
            }

            // Rotate input direction for custom gravity
            _movingDirection = Quaternion.FromToRotation(Vector3.up, UpDirection) * _movingDirection;
            _headingDirection = _movingDirection;

            // Decide to switch between acceleration and deceleration
            _isAccelerating = _horizontalInputsDetected;
        }

        /// <summary>
        /// Apply gravity to the character.
        /// </summary>
        private void HandlePlayerGravity()
        {
            _rb.AddForce(_rb.mass * _currentGravity);
        }

        /// <summary>
        /// Add torque to maintain the character vertical (according to its UpDirection, i.e. its groundRayDir)
        /// </summary>
        private void HandleTorqueRotation()
        {
            Vector3 desiredDirection =
                _movingDirection.magnitude < 0.1f ? Vector3.ProjectOnPlane(transform.forward, UpDirection) : _movingDirection;

            Quaternion desiredRotation = Quaternion.LookRotation(
                desiredDirection, 
                UpDirection);

            Quaternion characterCurrent = transform.rotation;
            Quaternion toGoal = MathUtils.ShortestRotation(characterCurrent, desiredRotation);

            toGoal.ToAngleAxis(out var rotDegrees, out var rotAxis);
            rotAxis.Normalize();

            float rotRadians = rotDegrees * Mathf.Deg2Rad;

            _rb.AddTorque((rotAxis * (rotRadians * jointSpringStrength)) -
                          (_rb.angularVelocity * jointSpringDamper), ForceMode.Acceleration);
        }

        /// <summary>
        /// Apply movements multipliers if any.
        /// </summary>
        private void HandleMovementMultipliers()
        {
            // Add multipliers handles here
            //
        }
        
        /// <summary>
        /// Handle Horizontal Movement by adding horizontal force.
        /// </summary>
        private void HandleHorizontalMovement()
        {
            // Velocity below character
            Vector3 groundVel = GetGroundVelocity();

            Vector3 unitVel = _targetVelocity.normalized;

            float velDot = Vector3.Dot(_movingDirection, unitVel);

            // Current horizontal velocity
            Vector3 horizontalVel = Vector3.ProjectOnPlane(_rb.velocity, UpDirection);

            // Mandatory Acceleration multipliers
            float acc = 1f;
            acc *= _isAccelerating ? acceleration : deceleration;
            acc *= accelerationFactorFromDot.Evaluate(velDot);

            // Add acceleration multipliers here
            //
            acc *= maxAccelerationMultiplier;

            // Calculate the goal velocity to achieve.
            Vector3 goalVel = maxSpeed *
                              maxSpeedMultiplier *
                              _movingDirection;

            // Calculate the target velocity for next frame.
            _targetVelocity = Vector3.MoveTowards(
                _rb.velocity, // TODO: understand why put targetVelocity and not RB here. Hint: for wallJump it has to be rb.velocity so the velocities are synchronised.
                (goalVel) + (groundVel) + (_externalVelocity),
                acc * Time.fixedDeltaTime);

            // Check to apply only an horizontal force.
            _targetVelocity = Vector3.ProjectOnPlane(_targetVelocity, UpDirection);

            // Calculate the force needed for the next movement.
            Vector3 neededAccel = (_targetVelocity - horizontalVel) / Time.fixedDeltaTime;

            // Special case for zero deceleration
            if (!_horizontalInputsDetected && deceleration == 0f) neededAccel = Vector3.zero;

            // Clamp max acceleration to limit the force applied by the player (prevent pushing over weight objects).
            float maxAccel;
            if (!_horizontalInputsDetected)
            {
                maxAccel = maxDecelerationForce;
            }
            else
            {
                maxAccel = maxAccelerationForce * maxAccelerationForceFactorFromDot.Evaluate(velDot);
            }
            neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

            // Add force
            _rb.AddForce(_rb.mass * neededAccel);
        }
    
        #endregion
    
        #region Update Handles

        /// <summary>
        /// Speed up character rotation.
        /// </summary>
        private void HandleRotation()
        {
            Vector3 hvel = Vector3.ProjectOnPlane(_rb.velocity, UpDirection);
        
            Vector3 desiredDirection =
                _movingDirection.magnitude < 0.1f ? Vector3.ProjectOnPlane(transform.forward, UpDirection) : _movingDirection;
        
            if (hvel.magnitude > 0.05f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(desiredDirection, UpDirection),
                    turnSmoothRatio);
            }
        }

        #endregion

        #region Tools

        /// <summary>
        /// Raycast to the ground and update the groundHit variable.
        /// Trigger the OnLanding and OnTakeOff events.
        /// </summary>
        private void DetectGround()
        {
            bool hasHit = Physics.Raycast(
                transform.position,
                groundRayDir,
                out _groundHit,
                groundRayLength,
                groundLayer,
                QueryTriggerInteraction.Ignore
            );

            if (!_groundDetected.Equals(hasHit))
            {
                if (hasHit && !_groundDetected)
                {
                    // Landing
                    OnLanding?.Invoke();

                    _jumps = 0;
                }

                if (!hasHit && _groundDetected)
                {
                    // Take off
                    OnTakeOff?.Invoke();
                }
            
                _groundDetected = hasHit;

                // Handle coyote delay
                if (_groundDetected)
                {
                    _groundCoyoteTime = 0f;
                }
                else
                {
                    _groundCoyoteTime += Time.fixedDeltaTime;
                }
            }

            // Debug.DrawLine(transform.position, transform.position + groundRayLength * Vector3.down, Color.black);
        }
    
        /// <summary>
        /// Detect if the player is sliding i.e. the slope below the character has an
        /// agle over the maxSlopeAngle.
        /// </summary>
        private void DetectSlope()
        {
            float slopeAngle = Vector3.Angle(groundRayDir, -_groundHit.normal);

            if (slopeAngle > maxSlopeAngle)
            {
                _isSliding = true;
            }
            else
            {
                _isSliding = false;
            }
        }

        private Vector3 GetGroundVelocity()
        {
            // Moving platforms
            if (_groundHit.collider != null)
            {
                if (_groundHit.rigidbody != null || _groundHit.collider.GetComponent<MovingPlatformComponent>() != null)
                {
                    // return _groundHit.rigidbody.GetPointVelocity(_groundHit.point);
                    return PhysicsUtils.GetRelativePointVelocity(transform, _groundHit.collider.gameObject, _groundHit.point);
                }
            }

            return Vector3.zero;
        }
        
        /// <summary>
        /// Raycast where the player is looking and update the wallHit variable.
        /// </summary>
        private void DetectWall()
        {
            Vector3 dir = MathUtils.EqualApprox(_headingDirection.magnitude, 0f)
                ? Vector3.ProjectOnPlane(transform.forward, UpDirection)
                : _headingDirection;
            
            bool hasHit = Physics.Raycast(
                transform.position,
                dir,
                out _wallHit,
                wallRayLength,
                wallLayer,
                QueryTriggerInteraction.Ignore
            );
            
            // Debug.DrawLine(transform.position, transform.position + wallRayLength * wallRayDir.normalized, Color.black);

            if (!_wallDetected && hasHit)
            {
                OnWallHit?.Invoke();
            }
            
            _wallDetected = hasHit;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Rotate the character to look towards the position given (hard rotation not based on forces.)
        /// Set the transform rotation.
        /// </summary>
        /// <param name="position"></param>
        public void LookTowards(Vector3 position)
        {
            transform.rotation = 
                Quaternion.LookRotation(Vector3.ProjectOnPlane(position - transform.position, UpDirection));
        }

        /// <summary>
        /// Set the rigidbody position of this character.
        /// </summary>
        /// <param name="newPosition"></param>
        public void SetPosition(Vector3 newPosition)
        {
            _rb.position = newPosition;
        }
        
        /// <summary>
        /// Set the velocity of this character. Prefer this method over directly setting the rigidbody's velocity.
        /// </summary>
        /// <param name="newVelocity"></param>
        public void SetVelocity(Vector3 newVelocity)
        {
            _targetVelocity = newVelocity;
            _rb.velocity = newVelocity;
        }

        /// <summary>
        /// Set the gravity for the character.
        /// Modify both the gravity force applied, and the groundRayDir if the new gravity is > 0.
        /// </summary>
        /// <param name="newGravity"></param>
        public void SetGravity(Vector3 newGravity)
        {
            if (MathUtils.MoreThanApprox(newGravity.magnitude, 0f))
            {
                groundRayDir = newGravity.normalized;
            }
            _currentGravity = newGravity;
        }

        #endregion
    }
}
