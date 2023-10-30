using System;
using UnityEngine;

namespace RBCC.Scripts.Player.PlayerStateData
{
    [Serializable]
    public class PlayerCrouchData
    {
        [Header("Collider")] 
        public float colliderHeight = 1.5f;
        public Vector3 colliderCenter = new Vector3(0f ,-0.25f, 0f);

        [Header("Floating")] 
        public float rideHeight = 1.25f;
        
        [Header("Horizontal movements")]
        public float maxSpeed = 8f;
        public float acceleration = 50f;
        public float maxAccelerationForce = 150f;
        public float deceleration = 50f;
        public float maxDecelerationForce = 150f;
    }
}
