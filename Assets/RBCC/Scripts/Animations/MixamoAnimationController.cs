using RBCC.Scripts.Player;
using UnityEngine;

namespace RBCC.Scripts.Animations
{
    public class MixamoAnimationController : MonoBehaviour
    {
        [SerializeField] private PlayerStateController player;
        [SerializeField] private Animator animatorController;
        
        private int _isRunningKey;
        private int _isSlidingKey;
        private int _isJumpingKey;
        
        private bool _isJumping;

        private void OnEnable()
        {
            _isRunningKey = Animator.StringToHash("isRunning");
            _isSlidingKey = Animator.StringToHash("isSliding");
            _isJumpingKey = Animator.StringToHash("isJumping");
            
            player.OnEnterState += UpdateEnterAnimationStates;
            player.OnExitState += UpdateExitAnimationStates;
        }

        private void OnDisable()
        {
            player.OnEnterState -= UpdateEnterAnimationStates;
            player.OnExitState -= UpdateExitAnimationStates;
        }

        private void UpdateEnterAnimationStates(PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.PlayerWalkState:
                case PlayerState.PlayerRunState:
                    animatorController.SetBool(_isRunningKey, true);
                    break;
                
                case PlayerState.PlayerSlideState:
                    animatorController.SetBool(_isSlidingKey, true);
                    player.groundRayLength += 1f; // To stick to the slide
                    break;
                
                case PlayerState.PlayerFallState:
                case PlayerState.PlayerJumpState:
                    if (!_isJumping)
                    {
                        _isJumping = true;
                        animatorController.SetBool(_isJumpingKey, true);
                    }
                    break;
                
                case PlayerState.PlayerGroundState:
                    _isJumping = false;
                    animatorController.SetBool(_isJumpingKey, false);
                    break;
            }
        }
        
        private void UpdateExitAnimationStates(PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.PlayerWalkState:
                case PlayerState.PlayerRunState:
                    animatorController.SetBool(_isRunningKey, false);
                    break;
                
                case PlayerState.PlayerSlideState:
                    animatorController.SetBool(_isSlidingKey, false);
                    player.groundRayLength -= 1f; // To stick to the slide
                    break;
            }
        }
    }
}
