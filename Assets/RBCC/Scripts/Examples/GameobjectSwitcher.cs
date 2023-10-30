using UnityEngine;
using UnityEngine.InputSystem;

namespace RBCC.Scripts.Examples
{
    public class GameobjectSwitcher : MonoBehaviour
    {
        public GameObject[] gameObjects;
    
        private int currentIndex = 0;

        private void Start()
        {
            if (gameObjects.Length < 1)
            {
                return;
            }
        
            foreach (GameObject o in gameObjects)
            {
                o.SetActive(false);
            }
            gameObjects[0].SetActive(true);
        }

        public void SwitchObject(InputAction.CallbackContext context)
        {
            if (context.performed && gameObjects.Length > 0)
            {
                gameObjects[currentIndex].SetActive(false);
                currentIndex += 1;
                currentIndex %= gameObjects.Length;
                gameObjects[currentIndex].SetActive(true);
            }
        }
    }
}
