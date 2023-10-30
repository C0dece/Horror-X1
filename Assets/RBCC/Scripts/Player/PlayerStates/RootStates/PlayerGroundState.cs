using RBCC.Scripts.Player.PlayerStateData;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RBCC.Scripts.Player.PlayerStates.RootStates
{
    /// <summary>
    /// State when the player is grounded.
    /// </summary>
    public class PlayerGroundState : PlayerBaseState
    {
        private readonly PlayerGroundData groundData;

        private bool _shouldFloat = true;

        #region Overrides

        public PlayerGroundState(PlayerStateController ctx, PlayerStateFactory factory, PlayerGroundData groundData) : base(ctx, factory)
        {
            IsRootState = true;
            InitializeSubState();
            this.groundData = groundData;
        }

        public override PlayerState State => PlayerState.PlayerGroundState;

        public override void EnterState()
        {
            // Register inputs
            Ctx.OnJumpPressed += OnJump;
            
            // Set Gravity to 0 while grounded to handle floating properly
            Ctx.SetGravity(Vector3.zero);

            // Set movement curves
            Ctx.accelerationFactorFromDot = groundData.accelerationFactorFromDot;
            Ctx.maxAccelerationForceFactorFromDot = groundData.maxAccelerationForceFactorFromDot;
            
            // Set rotation torque and smooth ratio
            Ctx.JointSpringStrength = groundData.jointSpringStrength;
            Ctx.JointSpringDamper = groundData.jointSpringDamper;
            Ctx.TurnSmoothRatio = groundData.turnSmoothRatio;
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void FixedUpdateState()
        {
            if (_shouldFloat)
            {
                Float();
            }
        }

        public override void ExitState()
        {
            // Unregister inputs
            Ctx.OnJumpPressed -= OnJump;
        }

        public override void CheckSwitchStates()
        {
            // Falling
            if (!Ctx.IsGrounded)
            {
                SwitchState(Factory.Air());
                return;
            }
        }

        public override void InitializeSubState()
        {
            // To Slide
            if (Ctx.IsSliding)
            {
                SetSubState(Factory.Slide());
                return;
            }
            else
            {
                // To Crouch
                if (Ctx.CrouchKeyPerformed)
                {
                    SetSubState(Factory.Crouch());
                    return;
                }
                else
                {
                    // To Idle
                    if (!Ctx.HorizontalInputsDetected)
                    {
                        SetSubState(Factory.Idle());
                        return;
                    }
            
                    // To Walk
                    if (Ctx.HorizontalInputsDetected && !Ctx.RunKeyPerformed)
                    {
                        SetSubState(Factory.Walk());
                        return;
                    }
            
                    // To Run
                    if (Ctx.HorizontalInputsDetected && Ctx.RunKeyPerformed)
                    {
                        SetSubState(Factory.Run());
                        return;
                    }
                }
            }
        }

        #endregion

        #region Methods

        private void Float()
        {
            Vector3 vel = Ctx.Rb.velocity;
            Vector3 otherVel = Vector3.zero; // ground below player velocity

            Rigidbody hitBody = Ctx.GroundHit.rigidbody;
            if (hitBody != null)
            {
                otherVel = hitBody.velocity;
            }

            float rayDirVel = Vector3.Dot(Ctx.groundRayDir, vel);
            float otherDirVel = Vector3.Dot(Ctx.groundRayDir, otherVel);
            float relVel = rayDirVel - otherDirVel;

            // Adjust ride height because of gravity
            float adjustedRideHeight = groundData.rideHeight;

            float x = Ctx.GroundHit.distance - adjustedRideHeight;
            float springForce = (x * groundData.rideSpringStrength) - (relVel * groundData.rideSpringDamper);

            // Debug.DrawLine(transform.position, transform.position + (rayDir * springForce), Color.yellow);

            Ctx.Rb.AddForce(Ctx.groundRayDir * springForce, ForceMode.Acceleration); // Ignore mass for floating

            if (hitBody != null)
            {
                hitBody.AddForceAtPosition(groundData.groundSpringMultiplier * -springForce * Ctx.groundRayDir, Ctx.GroundHit.point);
            }
        }
        
        private void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && Ctx.AllowJump)
            {
                if (Ctx.IsGrounded || Ctx.GroundCoyoteTime < Ctx.jumpCoyoteDelay)
                {
                    SwitchState(Factory.Air());
                    return;
                }
            }
        }

        #endregion

        // Getter and setter
        public float RideHeight
        {
            get => groundData.rideHeight;
            set => groundData.rideHeight = value;
        }

        public void EnableFloating(bool value)
        {
            _shouldFloat = value;
        }
    }
}