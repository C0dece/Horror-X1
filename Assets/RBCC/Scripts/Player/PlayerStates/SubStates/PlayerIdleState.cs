using RBCC.Scripts.Player.PlayerStateData;

namespace RBCC.Scripts.Player.PlayerStates.SubStates
{
    public class PlayerIdleState : PlayerBaseState
    {
        private readonly PlayerIdleData idleData;

        public PlayerIdleState(PlayerStateController ctx, PlayerStateFactory factory, PlayerIdleData idleData) : base(ctx, factory)
        {
            this.idleData = idleData;
        }

        public override PlayerState State => PlayerState.PlayerIdleState;

        public override void EnterState()
        {
            Ctx.maxSpeed = idleData.maxSpeed;
            Ctx.acceleration = idleData.acceleration;
            Ctx.maxAccelerationForce = idleData.maxAccelerationForce;
            Ctx.deceleration = idleData.deceleration;
            Ctx.maxDecelerationForce = idleData.maxDecelerationForce;
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
            // To Walk
            if (Ctx.HorizontalInputsDetected && !Ctx.CrouchKeyPerformed && !Ctx.IsSliding)
            {
                SwitchState(Factory.Walk());
                return;
            }
            
            // To Crouch
            if (Ctx.CrouchKeyPerformed)
            {
                SwitchState(Factory.Crouch());
            }
            
            // To Slide
            if (Ctx.IsSliding)
            {
                SwitchState(Factory.Slide());
                return;
            }
        }

        public override void InitializeSubState()
        {
        }
    }
}
