using RBCC.Scripts.Player.PlayerStateData;
using UnityEngine.InputSystem;

namespace RBCC.Scripts.Player.PlayerStates.SubStates
{
    public class PlayerWalkState : PlayerBaseState
    {
        private readonly PlayerWalkData walkData;

        public PlayerWalkState(PlayerStateController ctx, PlayerStateFactory factory, PlayerWalkData walkData) : base(ctx, factory)
        {
            this.walkData = walkData;
        }

        public override PlayerState State => PlayerState.PlayerWalkState;

        public override void EnterState()
        {
            Ctx.maxSpeed = walkData.maxSpeed;
            Ctx.acceleration = walkData.acceleration;
            Ctx.maxAccelerationForce = walkData.maxAccelerationForce;
            Ctx.deceleration = walkData.deceleration;
            Ctx.maxDecelerationForce = walkData.maxDecelerationForce;

            Ctx.OnDashPressed += OnDash;
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
            Ctx.OnDashPressed -= OnDash;
        }

        public override void CheckSwitchStates()
        {
            if (!Ctx.IsSliding)
            {
                // To Idle
                if (!Ctx.HorizontalInputsDetected)
                {
                    SwitchState(Factory.Idle());
                    return;
                }

                // To Run
                if (Ctx.HorizontalInputsDetected && Ctx.RunKeyPerformed)
                {
                    SwitchState(Factory.Run());
                    return;
                }
            
                // To crouch
                if (Ctx.CrouchKeyPerformed)
                {
                    SwitchState(Factory.Crouch());
                }
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

        private void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SwitchState(Factory.Dash());
            }
        }
    }
}
