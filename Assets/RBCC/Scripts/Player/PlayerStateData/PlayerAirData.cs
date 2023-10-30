using System;
using UnityEngine;

namespace RBCC.Scripts.Player.PlayerStateData
{
    [Serializable]
    public class PlayerAirData
    {
        [Header("Gravity")]
        public float gravityScale = 5f;
        
        [Header("Horizontal Movements")]
        public float maxSpeed = 0.8f;
        public float acceleration = 5f;
        public float maxAccelerationForce = 150f;
        public float deceleration = 50f;
        public float maxDecelerationForce = 150f;
        public AnimationCurve accelerationFactorFromDot;
        public AnimationCurve maxAccelerationForceFactorFromDot;
        
        [Header("Rotation")] 
        public float jointSpringStrength = 50f;
        public float jointSpringDamper = 5f;
        public float turnSmoothRatio = 0.05f;
    }
}