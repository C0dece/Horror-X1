using System;
using UnityEngine;

namespace RBCC.Scripts.Player.PlayerStateData
{
    [Serializable]
    public class PlayerWalkData
    {
        [Header("Horizontal movements")]
        public float maxSpeed = 8f;
        public float acceleration = 50f;
        public float maxAccelerationForce = 150f;
        public float deceleration = 50f;
        public float maxDecelerationForce = 150f;
    }
}
