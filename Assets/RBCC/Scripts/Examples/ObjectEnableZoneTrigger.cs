using UnityEngine;

namespace RBCC.Scripts.Examples
{
    public class ObjectEnableZoneTrigger : MonoBehaviour
    {
        public GameObject objectToEnableInZone;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                objectToEnableInZone.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                objectToEnableInZone.SetActive(false);
            }
        }
    }
}
