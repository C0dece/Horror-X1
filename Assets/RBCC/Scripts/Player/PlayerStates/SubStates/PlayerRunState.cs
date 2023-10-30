using RBCC.Scripts.Player.PlayerStateData;

namespace RBCC.Scripts.Player.PlayerStates.SubStates
{
    public class PlayerRunState : PlayerBaseState
    {
        private readonly PlayerRunData runData;

        public PlayerRunState(PlayerStateController ctx, PlayerStateFactory factory, PlayerRunData runData) : base(ctx, factory)
        {
            this.runData = runData;
        }

        public override PlayerState State => PlayerState.PlayerRunState;

        public override void EnterState()
        {
            Ctx.maxSpeed = runData.maxSpeed;
            Ctx.acceleration = runData.acceleration;
            Ctx.maxAccelerationForce = runData.maxAccelerationForce;
            Ctx.deceleration = runData.deceleration;
            Ctx.maxDecelerationForce = runData.maxDecelerationForce;
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
            // To Slide
            if (Ctx.IsSliding)
            {
                SwitchState(Factory.Slide());
                return;
            }
            else
            {
                // To walk
                if (!Ctx.HorizontalInputsDetected)
                {
                    SwitchState(Factory.Walk());
                    return;
                }
                if (!Ctx.RunKeyPerformed) // Releasing run key
                {
                    SwitchState(Factory.Walk());
                    return;
                }
                
                // To Crouch
                if (Ctx.CrouchKeyPerformed)
                {
                    SwitchState(Factory.Crouch());
                }
            }
        }

        public override void InitializeSubState()
        {
        }
    }
}
