using RBCC.Scripts.Player;
using UnityEngine;

namespace RBCC.Scripts.Environment.Zones
{
    public class SpeedZone : MonoBehaviour
    {
        [Tooltip("Set this to -1 to not apply any limit.")]
        public float speedLimit = 3f;
        [Tooltip("Set this to -1 to not apply any limit.")]
        public float accelerationLimit = 50f;
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerStateController player = other.GetComponent<PlayerStateController>();
                if (speedLimit >= 0f)
                {
                    float speedMultiplier = speedLimit / player.maxSpeed;
                    player.maxSpeedMultiplier = speedMultiplier;
                }

                if (accelerationLimit >= 0f)
                {
                    float accMultiplier = accelerationLimit / player.acceleration;
                    player.maxAccelerationMultiplier = accMultiplier;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerStateController player = other.GetComponent<PlayerStateController>();
                player.maxSpeedMultiplier = 1f;
                player.maxAccelerationMultiplier = 1f;
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 size = GetComponent<Collider>().bounds.size;
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawCube(transform.position, size);
        }
    }
}
