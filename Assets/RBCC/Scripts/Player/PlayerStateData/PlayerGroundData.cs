using System;
using UnityEngine;

namespace RBCC.Scripts.Player.PlayerStateData
{
    [Serializable]
    public class PlayerGroundData
    {
        [Header("Movement")]
        public AnimationCurve accelerationFactorFromDot;
        public AnimationCurve maxAccelerationForceFactorFromDot;
        
        [Header("Floating")] 
        public float rideHeight = 1.5f;
        public float rideSpringStrength = 100f;
        public float rideSpringDamper = 10f;
        [Tooltip("Multiplier on the force apply on the ground rigidbody. " +
                 "By default, the mass of the player is a good starting point.")]
        public float groundSpringMultiplier = 50f;

        [Header("Rotation")] 
        public float jointSpringStrength = 100f;
        public float jointSpringDamper = 10f;
        public float turnSmoothRatio = 0.05f;

        [Header("Slopes")] 
        public float maxSlopeAngle = 20f;
    }
}