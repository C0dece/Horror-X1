using System;
using RBCC.Scripts.Player.PlayerStateData;
using RBCC.Scripts.Player.PlayerStates;
using RBCC.Scripts.Player.PlayerStates.RootStates;
using RBCC.Scripts.Player.PlayerStates.SubStates;
using UnityEngine;

namespace RBCC.Scripts.Player
{
    /// <summary>
    /// Enum of all the possible player states. Useful to detect which state is active.
    /// Each state has a "State" attribute inherited from PlayerBaseState and overriden.
    /// </summary>
    public enum PlayerState
    {
        PlayerGroundState,
        PlayerAirState,
        PlayerIdleState,
        PlayerWalkState,
        PlayerRunState,
        PlayerCrouchState,
        PlayerSlideState,
        PlayerWallJumpState,
        PlayerJumpState,
        PlayerFallState,
        PlayerDashState,
    }
    
    /// <summary>
    /// Class responsible to create states.
    /// Each state is created at runtime to reset their values to the data one.
    /// You can disable a state (i.e. not be able to enter in it) by setting enableXState to false.
    /// This way, the state returned by this factory is null and the switch state doesn't happen.
    /// </summary>
    [Serializable]
    public class PlayerStateFactory
    {
        // Root data holders
        [SerializeField] private PlayerGroundData groundData;
        [SerializeField] private PlayerAirData airData;
        
        // Sub data holders
        [SerializeField] private PlayerIdleData idleData;
        [SerializeField] private PlayerWalkData walkData;
        [SerializeField] private PlayerRunData runData;
        [SerializeField] private PlayerSlideData slideData;
        [SerializeField] private PlayerCrouchData crouchData;
        [SerializeField] private PlayerJumpData jumpData;
        [SerializeField] private PlayerWallJumpData wallJumpData;
        [SerializeField] private PlayerFallData fallData;
        [SerializeField] private PlayerDashData dashData;
        
        // Enable State
        [SerializeField] private bool enableGroundState = true;
        [SerializeField] private bool enableAirState = true;
        [SerializeField] private bool enableIdleState = true;
        [SerializeField] private bool enableWalkState = true;
        [SerializeField] private bool enableRunState = true;
        [SerializeField] private bool enableSlideState = true;
        [SerializeField] private bool enableCrouchState = true;
        [SerializeField] private bool enableJumpState = true;
        [SerializeField] private bool enableWallJumpState = true;
        [SerializeField] private bool enableFallState = true;
        [SerializeField] private bool enableDashState = true;
        
        private PlayerStateController _context;

        public PlayerStateFactory(PlayerStateController context)
        {
            _context = context;
        }

        public void SetContext(PlayerStateController ctx)
        {
            _context = ctx;
        }

        #region Root States

        public PlayerBaseState Ground()
        {
            if (enableGroundState)
                return new PlayerGroundState(_context, this, groundData);
            return null;
        }
        
        public PlayerBaseState Air()
        {
            if (enableAirState)
                return new PlayerAirState(_context, this, airData);
            return null;
        }

        #endregion

        #region SubStates

        public PlayerBaseState Idle()
        {
            if (enableIdleState)
                return new PlayerIdleState(_context, this, idleData);
            return null;
        }
        
        public PlayerBaseState Walk()
        {
            if (enableWalkState)
                return new PlayerWalkState(_context, this, walkData);
            return null;
        }
        
        public PlayerBaseState Run()
        {
            if (enableRunState)
                return new PlayerRunState(_context, this, runData);
            return null;
        }
        
        public PlayerBaseState Slide()
        {
            if (enableSlideState)
                return new PlayerSlideState(_context, this, slideData);
            return null;
        }
        
        public PlayerBaseState Crouch()
        {
            if (enableCrouchState)
                return new PlayerCrouchState(_context, this, crouchData);
            return null;
        }
        
        public PlayerBaseState Jump()
        {
            if (enableJumpState)
                return new 
                    PlayerJumpState(_context, this, jumpData);
            return null;
        }
        
        public PlayerBaseState WallJump()
        {
            if (enableWallJumpState)
                return new 
                    PlayerWallJumpState(_context, this, wallJumpData);
            return null;
        }
        
        public PlayerBaseState Fall()
        {
            if (enableFallState)
                return new 
                    PlayerFallState(_context, this, fallData);
            return null;
        }
        
        // Add more here if you like

        public PlayerBaseState Dash()
        {
            if (enableDashState)
                return new 
                    PlayerDashState(_context, this, dashData);
            return null;
        }

        #endregion
    }
}
