using RBCC.Scripts.Player.PlayerStateData;
using UnityEngine;

namespace RBCC.Scripts.Player.PlayerStates.RootStates
{
    /// <summary>
    /// State when the player is in the air.
    /// </summary>
    public class PlayerAirState : PlayerBaseState
    {
        private readonly PlayerAirData airData;

        public PlayerAirState(PlayerStateController ctx, PlayerStateFactory factory, PlayerAirData airData) : base(ctx, factory)
        {
            IsRootState = true;
            this.airData = airData;
            InitializeSubState();
        }

        public override PlayerState State => PlayerState.PlayerAirState;

        public override void EnterState()
        {
            // Set up Gravity
            Ctx.SetGravity(airData.gravityScale * Physics.gravity.magnitude * -Ctx.UpDirection);
            
            Ctx.maxSpeed = airData.maxSpeed;
            Ctx.acceleration = airData.acceleration;
            Ctx.maxAccelerationForce = airData.maxAccelerationForce;
            Ctx.deceleration = airData.deceleration;
            Ctx.maxDecelerationForce = airData.maxDecelerationForce;
            Ctx.accelerationFactorFromDot = airData.accelerationFactorFromDot;
            Ctx.maxAccelerationForceFactorFromDot = airData.maxAccelerationForceFactorFromDot;
            
            // Set rotation torque and smooth ratio
            Ctx.JointSpringStrength = airData.jointSpringStrength;
            Ctx.JointSpringDamper = airData.jointSpringDamper;
            Ctx.TurnSmoothRatio = airData.turnSmoothRatio;
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
        }

        public override void CheckSwitchStates()
        {
            if (Ctx.IsGrounded && !Ctx.IsJumpingUp)
            {
                SwitchState(Factory.Ground());
            }
        }

        public override void InitializeSubState()
        {
            // Falling
            if (!Ctx.IsGrounded)
            {
                SetSubState(Factory.Fall());
                return;
            }
            
            // Default: jump
            if (Ctx.AllowJump)
                SetSubState(Factory.Jump());
        }
    }
}
