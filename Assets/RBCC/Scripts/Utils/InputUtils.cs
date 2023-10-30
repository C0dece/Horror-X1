using System.Linq;
using UnityEngine.InputSystem;

namespace RBCC.Scripts.Utils
{
    public enum InputInteraction
    {
        Hold,
        MultiTap,
        Press,
        SlowTap,
        Tap
    }
    
    public class InputUtils
    {
        public static bool ContainsInteraction(InputAction action, InputInteraction interaction)
        {
            string[] strInteractions = action.interactions.Split(',');
            switch (interaction)
            {
                case InputInteraction.Hold:
                    return strInteractions.Contains("Hold");
                case InputInteraction.MultiTap:
                    return strInteractions.Contains("MultiTap");
                case InputInteraction.Press:
                    return strInteractions.Contains("Press");
                case InputInteraction.SlowTap:
                    return strInteractions.Contains("SlowTap");
                case InputInteraction.Tap:
                    return strInteractions.Contains("Tap");
                default:
                    return false;
            }
        }
    }
}
