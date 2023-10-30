using UnityEngine;

namespace RBCC.Scripts
{
    public class BuildDebugger : MonoBehaviour
    {
        public static BuildDebugger Instance;

        public bool enableVSync;
        public int targetFramerate = 60;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            QualitySettings.vSyncCount = enableVSync ? 1 : 0;
            Application.targetFrameRate = targetFramerate;
        }
    }
}
