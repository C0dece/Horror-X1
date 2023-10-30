using RBCC.Scripts.Player.PlayerStateData;
using UnityEngine;

namespace RBCC.Scripts.Player.PlayerStates.SubStates
{
    public class PlayerWallJumpState : PlayerBaseState
    {
        private readonly PlayerWallJumpData wallJumpData;
        
        private float _jumpHoldTime; // Time before reaching peak jump, used to jump lower when releasing jump key
        private float _jumpTime; // Current time from the start of the jump

        public PlayerWallJumpState(PlayerStateController ctx, PlayerStateFactory factory, PlayerWallJumpData wallJumpData) : base(ctx, factory)
        {
            this.wallJumpData = wallJumpData;
        }

        public override PlayerState State => PlayerState.PlayerWallJumpState;

        public override void EnterState()
        {
            // Get direction of the jump
            Vector3 rotAxis = Vector3.Cross(Ctx.UpDirection, Ctx.WallHit.normal);
            Vector3 jumpDir = Quaternion.AngleAxis(-90f + wallJumpData.verticalAngle, rotAxis) * Ctx.WallHit.normal;
            
            // Debug.DrawRay(Ctx.transform.position, jumpDir * 3f);
            
            WallJump(jumpDir, wallJumpData.jumpHeight, wallJumpData.verticalAngle);
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void FixedUpdateState()
        {
        }

        public override void ExitState()
        {
            Ctx.IsJumpingUp = false; // Just to be sure to reset
        }

        public override void CheckSwitchStates()
        {
            // Just wall jump and exit
            SwitchState(Factory.Fall());
        }

        public override void InitializeSubState()
        {
        }

        private void WallJump(Vector3 dir, float verticalHeight, float verticalAngle)
        {
            float distance = verticalHeight; // / Mathf.Cos(wallJumpData.verticalAngle * Mathf.Deg2Rad);
            
            // Calculate force to jump on the given height
            float yForce = Ctx.Rb.mass * Mathf.Sqrt(verticalHeight * 2 * Ctx.CurrentGravity.magnitude); // Gravity during jump
            float xForce = Mathf.Tan(verticalAngle * Mathf.Deg2Rad) * Mathf.Sqrt(verticalHeight * 2 * Ctx.CurrentGravity.magnitude);
            float jumpForce = Mathf.Sqrt(xForce * xForce + yForce * yForce);
            
            // Cancel current vertical velocity
            Vector3 currentVelocity = Ctx.Rb.velocity;
            Ctx.Rb.velocity = Vector3.ProjectOnPlane(currentVelocity, Ctx.UpDirection);
        
            // Add vertical force to jump
            Ctx.Rb.AddForce(jumpForce * dir, ForceMode.Impulse);
        }
    }
}
