using RBCC.Scripts.Player.PlayerStateData;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RBCC.Scripts.Player.PlayerStates.SubStates
{
    public class PlayerFallState : PlayerBaseState
    {
        private readonly PlayerFallData fallData;

        public PlayerFallState(PlayerStateController ctx, PlayerStateFactory factory, PlayerFallData fallData) : base(ctx, factory)
        {
            this.fallData = fallData;
        }

        public override PlayerState State => PlayerState.PlayerFallState;

        public override void EnterState()
        {
            // Register inputs
            Ctx.OnJumpPressed += OnJump;
            
            Ctx.Jumps = (int)Mathf.Clamp(Ctx.Jumps, 1f, Mathf.Infinity);
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
            Ctx.OnJumpPressed -= OnJump;
        }

        public override void CheckSwitchStates()
        {
        }

        public override void InitializeSubState()
        {
        }
        
        private void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && Ctx.AllowJump)
            {
                // Check for wall jump first
                if (Ctx.WallDetected)
                {
                    SwitchState(Factory.WallJump());
                    return;
                }

                // Double / Multiple jump
                if (Ctx.Jumps < Ctx.maxJumps)
                {
                    SwitchState(Factory.Jump());
                    return;
                }
            }
        }
    }
}
