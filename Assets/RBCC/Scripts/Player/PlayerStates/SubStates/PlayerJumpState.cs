using RBCC.Scripts.Player.PlayerStateData;
using RBCC.Scripts.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RBCC.Scripts.Player.PlayerStates.SubStates
{
    public class PlayerJumpState : PlayerBaseState
    {
        private readonly PlayerJumpData jumpData;
        
        private float _jumpHoldTime; // Time before reaching peak jump, used to jump lower when releasing jump key
        private float _jumpTime; // Current time from the start of the jump

        public PlayerJumpState(PlayerStateController ctx, PlayerStateFactory factory, PlayerJumpData jumpData) : base(ctx, factory)
        {
            this.jumpData = jumpData;
        }

        public override PlayerState State => PlayerState.PlayerJumpState;

        public override void EnterState()
        {
            // Register inputs
            Ctx.OnJumpPressed += CancelJump;
            
            Jump(jumpData.jumpHeight);
            Ctx.Jumps++;
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
            
            HandleJumpTime();
        }

        public override void FixedUpdateState()
        {
        }

        public override void ExitState()
        {
            // Unregister inputs
            Ctx.OnJumpPressed -= CancelJump;
            
            Ctx.IsJumpingUp = false; // Just to be sure to reset
        }

        public override void CheckSwitchStates()
        {
            // To Fall (after realeasing jump key basically)
            if (!Ctx.IsJumpingUp)
            {
                SwitchState(Factory.Fall());
                return;
            }
        }

        public override void InitializeSubState()
        {
        }

        private void Jump(float height)
        {
            // Cancel any y-velocity when grounded. (Especially with floating that can induces y-velocity, so the jump could be lower)
            float yVelCancelRate = Ctx.Jumps == 0 ? 1f : jumpData.velCancelRate;
            
            // Calculate force to jump on the given height
            float jumpForce = Ctx.Rb.mass * Mathf.Sqrt(height * 2 * Ctx.CurrentGravity.magnitude); // Gravity during jump
            float timeToPeak = Mathf.Sqrt(2 * height / Ctx.CurrentGravity.magnitude);
        
            // Cancel current vertical velocity (if the vel is against UpDir)
            Vector3 currentVelocity = Ctx.Rb.velocity;
            if (Vector3.Dot(currentVelocity, Ctx.UpDirection) < 0f)
            {
                Vector3 yVel = Vector3.Project(currentVelocity, Ctx.UpDirection) * Mathf.Clamp01(1f - yVelCancelRate);
                Vector3 newVel = Vector3.ProjectOnPlane(currentVelocity, Ctx.UpDirection) + yVel;
                Ctx.Rb.velocity = newVel;
            }
            
        
            // Add vertical force to jump
            Ctx.Rb.AddForce(jumpForce * Ctx.UpDirection, ForceMode.Impulse);
        
            // Set up jump hold time for cancelling jump
            _jumpHoldTime = timeToPeak;
            _jumpTime = 0f;
            Ctx.IsJumpingUp = true;
        }

        private void HandleJumpTime()
        {
            // Handle jump time
            if (Ctx.IsJumpingUp)
            {
                _jumpTime += Time.deltaTime;
                if (_jumpTime > _jumpHoldTime ||
                    MathUtils.LessThanApprox(Vector3.Dot(Vector3.Project(Ctx.Rb.velocity, Ctx.UpDirection), Ctx.UpDirection), 0f)) // Going downwards
                {
                    Ctx.IsJumpingUp = false;
                }
            }
        }

        /// <summary>
        /// Cancel force when releasing the jump key.
        /// This makes the mario jump effect (lower jump on instant press, longer jump when holding)
        /// Note: if you use the same input for double jump, this applies as well.
        /// </summary>
        /// <param name="context"></param>
        private void CancelJump(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                Vector3 cancelForce = jumpData.jumpCancelRate * Ctx.Rb.mass * -Vector3.Project(Ctx.Rb.velocity, Ctx.groundRayDir);
                Ctx.Rb.AddForce(cancelForce, ForceMode.Impulse);
            }
        }
    }
}
