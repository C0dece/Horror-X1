using RBCC.Scripts.Player.PlayerStateData;
using RBCC.Scripts.Player.PlayerStates.RootStates;
using UnityEngine;

namespace RBCC.Scripts.Player.PlayerStates.SubStates
{
    public class PlayerCrouchState : PlayerBaseState
    {
        private readonly PlayerCrouchData data;

        private CapsuleCollider _collider;
        
        private float _enterColliderHeight;
        private Vector3 _enterColliderCenter;
        private float _enterRideHeight;

        private bool _hasCeiling;

        public PlayerCrouchState(PlayerStateController ctx, PlayerStateFactory factory, PlayerCrouchData crouchData) : base(ctx, factory)
        {
            this.data = crouchData;
        }

        public override PlayerState State => PlayerState.PlayerCrouchState;

        public override void EnterState()
        {
            // Set up collider
            _collider = Ctx.GetComponent<CapsuleCollider>();
            _enterColliderCenter = _collider.center;
            _enterColliderHeight = _collider.height;
            _collider.center = data.colliderCenter;
            _collider.height = data.colliderHeight;
            
            // Set up floating
            if (CurrentSuperState is PlayerGroundState)
            {
                _enterRideHeight = ((PlayerGroundState) CurrentSuperState).RideHeight;
                ((PlayerGroundState) CurrentSuperState).RideHeight = data.rideHeight;
            }

            // Set up movements
            Ctx.maxSpeed = data.maxSpeed;
            Ctx.acceleration = data.acceleration;
            Ctx.maxAccelerationForce = data.maxAccelerationForce;
            Ctx.deceleration = data.deceleration;
            Ctx.maxDecelerationForce = data.maxDecelerationForce;
            
            // Disable jump while crouched
            Ctx.AllowJump = false;
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void FixedUpdateState()
        {
            CheckCeiling();
        }

        public override void ExitState()
        {
            // Set up collider
            _collider.center = _enterColliderCenter;
            _collider.height = _enterColliderHeight;
            
            // Reset floating
            if (CurrentSuperState is PlayerGroundState)
            {
                ((PlayerGroundState) CurrentSuperState).RideHeight = _enterRideHeight;
            }

            // Reset jump
            Ctx.AllowJump = true;
        }

        public override void CheckSwitchStates()
        {
            // Debug.Log(_hasCeiling);
            // Cancel exit state if there's a ceiling above the player
            if (_hasCeiling)
            {
                return;
            }
            
            if (!Ctx.CrouchKeyPerformed && !Ctx.IsSliding)
            {
                // To idle
                if (!Ctx.HorizontalInputsDetected)
                {
                    SwitchState(Factory.Idle());
                    return;
                }
                
                // To Walk
                if (Ctx.HorizontalInputsDetected)
                {
                    SwitchState(Factory.Walk());
                    return;
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

        /// <summary>
        /// Raycast above the character to prevent exiting crouching.
        /// </summary>
        private void CheckCeiling()
        {
            float rayLength = (_enterRideHeight + _enterColliderHeight) - (data.rideHeight + _collider.height / 2f);
            RaycastHit ceilingHit;
            _hasCeiling = Physics.Raycast(
                 Ctx.transform.TransformPoint(_collider.center),
                Ctx.UpDirection,
                out ceilingHit,
                rayLength,
                Ctx.groundLayer,
                QueryTriggerInteraction.Ignore
            );
        }
    }
}
