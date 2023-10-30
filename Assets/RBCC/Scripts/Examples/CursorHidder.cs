using UnityEngine;

namespace RBCC.Scripts.Examples
{
    public class CursorHidder : MonoBehaviour
    {
        public bool shouldShowCursor = false;

        private void Update()
        {
            Cursor.visible = shouldShowCursor;
            Cursor.lockState = shouldShowCursor ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
