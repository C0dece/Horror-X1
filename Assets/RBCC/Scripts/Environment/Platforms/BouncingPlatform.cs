using RBCC.Scripts.Player;
using RBCC.Scripts.Player.PlayerStates.RootStates;
using UnityEngine;

namespace RBCC.Scripts.Environment.Platforms
{
    public class BouncingPlatform : MonoBehaviour
    {
        public float force;
        public Rigidbody platform;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                PlayerStateController player = other.GetComponent<PlayerStateController>();
                (player.CurrentRootState as PlayerGroundState)?.EnableFloating(false);
            
                // Cancel vertical velocity
                Vector3 vel = player.CurrentVelocity;
                vel.Scale(new Vector3(1f, 0f, 1f));
                player.SetVelocity(vel);
            
                player.Rb.AddForce(force * transform.up);
                platform.AddForce(force * -transform.up);
            }
        }
    }
}