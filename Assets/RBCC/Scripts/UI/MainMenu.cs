using UnityEngine;
using UnityEngine.SceneManagement;

namespace RBCC.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        public int sceneIndexToPlay = 1;
    
        public void PlayGame()
        {
            SceneManager.LoadScene(sceneIndexToPlay);
        }
    }
}
