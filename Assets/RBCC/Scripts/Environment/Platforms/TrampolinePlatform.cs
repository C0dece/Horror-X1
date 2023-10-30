using System.Collections;
using RBCC.Scripts.Player;
using RBCC.Scripts.Player.PlayerStates.RootStates;
using UnityEngine;

namespace RBCC.Scripts.Environment.Platforms
{
    /// <summary>
    /// Platform that throws the player to the landing point if he is not moving while in air.
    /// </summary>
    public class TrampolinePlatform : MonoBehaviour
    {
        [SerializeField] private float throwDirectionAngle = 70f;
    
        [SerializeField] private Vector3 pointToLand = Vector3.forward; // Relative to its transform
    
        [SerializeField] [Tooltip("Rotate the character to face the throwing direction.")] 
        private bool isRotationControlled = true;

        [SerializeField] [Tooltip("Gravity to apply while in air.")]
        private Vector3 gravityInAir = Physics.gravity;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                PlayerStateController player = other.GetComponent<PlayerStateController>();
                Vector3 playerPos = player.transform.position;

                Vector3 horizontalTrajectory = Vector3.ProjectOnPlane(((transform.position + pointToLand) - playerPos),
                    Vector3.up);
            
                float distance = horizontalTrajectory.magnitude + 2f; // Offset for precision loss with forces applied

                Vector3 direction = Quaternion.AngleAxis(throwDirectionAngle, Vector3.Cross(horizontalTrajectory, Vector3.up)) *
                                    horizontalTrajectory;
            
                Vector3 throwVelocity = Mathf.Sqrt(gravityInAir.magnitude * distance / 
                                                   (2 * Mathf.Pow(Mathf.Cos(throwDirectionAngle * Mathf.Deg2Rad), 2) 
                                                      * Mathf.Tan(throwDirectionAngle * Mathf.Deg2Rad))) * 
                                        direction.normalized;
            
                float timeToPeak = distance / (2 * throwVelocity.magnitude * Mathf.Cos(throwDirectionAngle * Mathf.Deg2Rad));

                // Launch the player in the air
                player.SetVelocity(throwVelocity);
            
                // Disable movements for the time of the jump
                // by setting acceleration to basically 0
                StartCoroutine(DisableMovementsWhileInAir());
                IEnumerator DisableMovementsWhileInAir()
                {
                    (player.CurrentRootState as PlayerGroundState)?.EnableFloating(false);
                
                    // Set player.maxAccelerationMultiplier to 0 is a way to disable player's movement in air.
                    // player.maxAccelerationMultiplier = 0f;
                
                    yield return new WaitForSeconds(timeToPeak);
                
                    // player.maxAccelerationMultiplier = 1f;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw throw line direction
            Gizmos.color = Color.yellow;
        
            Vector3 horizontalTrajectory = Vector3.ProjectOnPlane(((transform.position + pointToLand) - transform.position),
                Vector3.up);
        
            Vector3 direction = Quaternion.AngleAxis(throwDirectionAngle, Vector3.Cross(horizontalTrajectory, Vector3.up)) *
                                horizontalTrajectory;
        
            Gizmos.DrawLine(transform.position, transform.position + direction.normalized * 5f);
        
            // Draw landing point sphere
            Gizmos.DrawSphere((transform.position + pointToLand), 1f);
        
            // Control rotation if enabled
            if (isRotationControlled)
            {
                Vector3 lookingDirection = Quaternion.AngleAxis(
                                               throwDirectionAngle + 90f, 
                                               Vector3.Cross(horizontalTrajectory, Vector3.up)) *
                                           horizontalTrajectory;
                transform.rotation = Quaternion.LookRotation(lookingDirection, Vector3.up);
            }
        }
    }
}
