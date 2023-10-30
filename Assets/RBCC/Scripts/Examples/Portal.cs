using UnityEngine;
using UnityEngine.SceneManagement;

namespace RBCC.Scripts.Examples
{
    public class Portal : MonoBehaviour
    {
        public string sceneNameToLoad;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SceneManager.LoadScene(sceneNameToLoad);
            }
        }
    }
}
