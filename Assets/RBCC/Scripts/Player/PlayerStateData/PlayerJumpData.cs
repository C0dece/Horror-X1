using System;
using UnityEngine;

namespace RBCC.Scripts.Player.PlayerStateData
{
    [Serializable]
    public class PlayerJumpData
    {
        [Header("Jump")]
        public float jumpHeight = 3f;
        [Tooltip("% of the vertical velocity to cancel when releasing jump key before jump peek height.")]
        [Range(0f, 1f)] public float jumpCancelRate = 0.5f; // How high the y velocity slows when cancelling jump
        [Tooltip("% of the vertical velocity to cancel when pressing jump key.")]
        [Range(0f, 1f)] public float velCancelRate = 0.9f;
    }
}
