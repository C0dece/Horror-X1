using RBCC.Scripts.Player.PlayerStateData;
using UnityEngine;

namespace RBCC.Scripts.Player.PlayerStates.SubStates
{
    public class PlayerDashState : PlayerBaseState
    {
        private readonly PlayerDashData data;
    
        public PlayerDashState(PlayerStateController ctx, PlayerStateFactory factory, PlayerDashData dashData) : base(ctx, factory)
        {
            this.data = dashData;
        }

        public override PlayerState State => PlayerState.PlayerDashState;
    
        public override void EnterState()
        {
            Ctx.Rb.AddForce(data.dashForce * Ctx.LookingDirection, ForceMode.Impulse);
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
            SwitchState(Factory.Idle());
        }

        public override void InitializeSubState()
        {
        
        }
    }
}
