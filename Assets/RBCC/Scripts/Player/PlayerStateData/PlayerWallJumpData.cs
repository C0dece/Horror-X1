using System;
using UnityEngine;

namespace RBCC.Scripts.Player.PlayerStateData
{
    [Serializable]
    public class PlayerWallJumpData
    {
        [Header("WallJump")] 
        [Tooltip("Angle in degrees from the top of the wall.")]
        [Range(1f, 45f)] // If above, weird behavior but you can try :)
        public float verticalAngle = 20f;
        [Tooltip("Vertical height to reach")] 
        public float jumpHeight = 3f;
    }
}
