using UnityEditor;
using UnityEngine;

namespace RBCC.Scripts.Utils
{
    public class GizmosUtils
    {
#if UNITY_EDITOR
        public static void DrawThickLine(Vector3 startPosition, Vector3 endPosition, float thickness = 1, Color color = default)
        {
            if (color == default)
            {
                color = Color.black;
            }
            Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, color, null, thickness);
        }

        public static void DrawCircle(Vector3 center, Vector3 normal, float radius, Color color = default)
        {
            if (color == default)
            {
                color = Color.black;
            }

            Handles.color = color;
            Handles.DrawWireDisc(
                center,  // position
                normal , // normal
                radius); // radius
        }
        
#endif
    }

}
