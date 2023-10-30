using System;
using UnityEngine;

namespace RBCC.Scripts.Player.PlayerStateData
{
    [Serializable]
    public class PlayerRunData
    {
        [Header("Horizontal Movements")]
        public float maxSpeed = 12f;
        public float acceleration = 60f;
        public float maxAccelerationForce = 150f;
        public float deceleration = 60f;
        public float maxDecelerationForce = 150f;
    }
}
