using UnityEngine;

namespace RBCC.Scripts.Examples
{
    public class RespawnPlane : MonoBehaviour
    {
        [SerializeField] private Vector3 respawnPoint = new Vector3(0f, 2f, 0f);
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null) rb.velocity = Vector3.zero;
                other.transform.position = respawnPoint;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawCube(transform.position, transform.localScale);
        
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(respawnPoint, 0.25f);
        }
    }
}
