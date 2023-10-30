namespace RBCC.Scripts.Player.PlayerStates
{
    /// <summary>
    /// Abstract class to implement a state.
    /// </summary>
    public abstract class PlayerBaseState
    {
        private PlayerStateController _ctx;
        private PlayerStateFactory _factory;
        private PlayerBaseState _currentSuperState;
        private PlayerBaseState _currentSubState;
        private bool _isRootState = false;
        
        // Properties
        protected bool IsRootState
        {
            get => _isRootState;
            set => _isRootState = value;
        }
        
        public abstract PlayerState State { get; }
        
        protected PlayerStateController Ctx => _ctx;
        protected PlayerStateFactory Factory => _factory;

        public PlayerBaseState CurrentSuperState => _currentSuperState;

        public PlayerBaseState CurrentSubState => _currentSubState;

        // Constructor
        protected PlayerBaseState(PlayerStateController ctx, PlayerStateFactory factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        // Abstracts
        public abstract void EnterState();
        
        public abstract void UpdateState();
        
        public abstract void FixedUpdateState();
        
        public abstract void ExitState();
        
        public abstract void CheckSwitchStates();
        
        /// <summary>
        /// Set the right subState when the super state is set.
        /// </summary>
        public abstract void InitializeSubState();

        // Protected
        
        /// <summary>
        /// Switch to a new state.
        /// If you call this function from a rootState, it will replace it.
        /// If you call it from a subState, it replaces the subState.
        /// So be careful to NOT replace a subState with a rootState.
        /// If you try to change to a substate without having one already set up, this does nothing.
        /// Do nothing if the newState is null.
        /// </summary>
        /// <param name="newState"></param>
        protected void SwitchState(PlayerBaseState newState)
        {
            if (newState == null)
            {
                return;
            }

            ExitStates();

            if (_isRootState)
            {
                // Replace root state.
                _ctx.CurrentRootState = newState;
            } 
            else if (_currentSuperState != null)
            {
                // Replace the subState.
                _currentSuperState.SetSubState(newState);
            }

            newState.EnterStates();
            
            // Debug.Log(newState.ToString());
        }
        
        /// <summary>
        /// Use SwitchState preferably.
        /// </summary>
        /// <param name="newSuperState"></param>
        protected void SetSuperState(PlayerBaseState newSuperState)
        {
            if (newSuperState == null)
            {
                return;
            }

            _currentSuperState = newSuperState;
        }

        /// <summary>
        /// Use SwitchState preferably.
        /// </summary>
        /// <param name="newSubState"></param>
        protected void SetSubState(PlayerBaseState newSubState)
        {
            if (newSubState == null)
            {
                return;
            }

            _currentSubState = newSubState;
            newSubState.SetSuperState(this);
        }

        protected PlayerBaseState GetRootState()
        {
            if (_currentSuperState == null)
            {
                return this;
            }
            else
            {
                return _currentSuperState.GetRootState();
            }
        }
        
        // Public

        /// <summary>
        /// Enter this state and all substates from it.
        /// </summary>
        public void EnterStates()
        {
            // Inform that a new state has been set.
            Ctx.OnEnterState?.Invoke(State);
            
            EnterState();
            _currentSubState?.EnterStates();
        }

        /// <summary>
        /// Exit this state and all of its substates.
        /// </summary>
        public void ExitStates()
        {
            // Inform that a new state has been set.
            Ctx.OnExitState?.Invoke(State);
            
            ExitState();
            _currentSubState?.ExitStates();
        }
        
        /// <summary>
        /// Update this state and the subStates if any on the update call.
        /// </summary>
        public void UpdateStates()
        {
            // Update only the current states
            if (_ctx.CurrentRootState == GetRootState())
            {
                UpdateState();
                _currentSubState?.UpdateStates();
            }
        }
        
        /// <summary>
        /// Update this state and the subStates if any on the fixed update call.
        /// </summary>
        public void FixedUpdateStates()
        {
            // Update only the current states
            if (_ctx.CurrentRootState == GetRootState())
            {
                FixedUpdateState();
                _currentSubState?.FixedUpdateStates();
            }
        }
    }
}
