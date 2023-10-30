using RBCC.Scripts.Player;
using UnityEngine;

namespace RBCC.Scripts.Animations
{
    public class CapsuleAnimationController : MonoBehaviour
    {
        [SerializeField] private PlayerStateController player;

        [Header("GFX")] 
        [SerializeField] private Transform capsule;
        [SerializeField] private Transform eyes;

        [Header("VFX")] 
        [SerializeField] private ParticleSystem landingParticles;
        [SerializeField] private ParticleSystem jumpingParticles;
        [SerializeField] private ParticleSystem runningParticles;
        [SerializeField] private ParticleSystem wallJumpingParticles;

        private void OnEnable()
        {
            player.OnEnterState += UpdateEnterAnimationStates;
            player.OnExitState += UpdateExitAnimationStates;
            player.OnLanding += LandingAnimation;
        }

        private void OnDisable()
        {
            player.OnEnterState -= UpdateEnterAnimationStates;
            player.OnExitState -= UpdateExitAnimationStates;
            player.OnLanding -= LandingAnimation;
        }

        private void Update()
        {
            // Ground particles
            if (player.IsGrounded && player.CurrentVelocity.magnitude > 1f)
            { 
                if (!runningParticles.isPlaying)
                {
                    runningParticles.Emit(1); // Workaround when waiting for the loop to begin.
                    runningParticles.Play();
                }
            }
            else
            {
                if (runningParticles.isPlaying)
                {
                    runningParticles.Stop();
                }
            }
        }

        private void UpdateEnterAnimationStates(PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.PlayerCrouchState:
                    // Hard coded
                    capsule.localScale = new Vector3(1f, 0.75f, 1f);
                    capsule.localPosition = new Vector3(0f, -0.25f, 0f);
                    eyes.localPosition = new Vector3(0f, 0.08f, 0f);
                    break;

                case PlayerState.PlayerJumpState:
                    jumpingParticles.Play();
                    break;
                
                case PlayerState.PlayerWallJumpState:
                    wallJumpingParticles.Play();
                    break;
            }
        }
        
        private void UpdateExitAnimationStates(PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.PlayerCrouchState:
                    // Hard coded
                    capsule.localScale = new Vector3(1f, 1f, 1f);
                    capsule.localPosition = new Vector3(0f, 0f, 0f);
                    eyes.localPosition = new Vector3(0f, 0.58f, 0f);
                    break;
            }
        }

        private void LandingAnimation()
        {
            landingParticles.Play();
        }
    }
}
