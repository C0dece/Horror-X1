using UnityEditor;

namespace RBCC.Scripts.Player.Editor
{
    [CustomEditor(typeof(PlayerStateController))]
    public class PlayerStateControllerEditor : UnityEditor.Editor
    {
        private PlayerStateController _stateController;
    
        private void OnEnable()
        {
            _stateController = (PlayerStateController) target;
        }
    
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        
            if (EditorApplication.isPlaying)
            {
                EditorGUI.BeginDisabledGroup(true);

                EditorGUILayout.TextField(_stateController.CurrentRootState.ToString());
                EditorGUILayout.TextField(_stateController.CurrentRootState.CurrentSubState.ToString());
            
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}
