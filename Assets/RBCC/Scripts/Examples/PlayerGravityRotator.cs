using System;
using RBCC.Scripts.Player;
using UnityEngine;

namespace RBCC.Scripts.Examples
{
    /// <summary>
    /// Rotate the player current gravity to the customGravityDirection.
    /// </summary>
    public class PlayerGravityRotator : MonoBehaviour
    {
        [SerializeField] private Vector3 customGravityDirection = Vector3.down;
        public bool resetGravityOnExit;
        public bool updateGravityDynamically;
    
        private Vector3 _exitGravity;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerStateController player = other.GetComponent<PlayerStateController>();

                _exitGravity = player.CurrentGravity;
            
                RotatePlayerGravity(player);
            }
        }

        /// <summary>
        /// Useful here if the object is rotating and you want to update gravity dynamically.
        /// </summary>
        /// <param name="other"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") && updateGravityDynamically)
            {
                PlayerStateController player = other.GetComponent<PlayerStateController>();

                RotatePlayerGravity(player);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Reset player gravity (as when it was when he entered)
            if (resetGravityOnExit && other.CompareTag("Player"))
            {
                PlayerStateController player = other.GetComponent<PlayerStateController>();

                // Set the ray dir and gravity to the ground direction
                player.SetGravity(_exitGravity);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.TransformDirection(customGravityDirection.normalized) * 10f);
        }

        private void RotatePlayerGravity(PlayerStateController player)
        {
            // Set the ray dir and gravity to the ground direction
            float currentGravityValue = player.CurrentGravity.magnitude;
            if (currentGravityValue > 0f)
            {
                Vector3 newGravity = transform.TransformDirection(customGravityDirection.normalized)
                                     * player.CurrentGravity.magnitude;
                player.SetGravity(newGravity);
            }
            else
            {
                Vector3 newRayDir = transform.TransformDirection(customGravityDirection.normalized);
                player.groundRayDir = newRayDir;
            }
        }
    }
}
