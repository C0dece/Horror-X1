using System;
using UnityEngine;

namespace RBCC.Scripts.Player.PlayerStateData
{
    [Serializable]
    public class PlayerSlideData
    {
        [Header("Slide")]
        [Tooltip("Force applied to push the character downwards.")]
        public float slideForce = 12f;
        [Tooltip("Step to increase the slideForce on each frame (*deltatime).")]
        public float slideForceAcc = 2 * 9.81f;
        [Tooltip("Torque applied to rotate the character downwards.")]
        public float slideTorque = 20f;
        [Tooltip("Step to increase the slideTorque on each frame (*deltatime).")]
        public float slideTorqueAcc = 2 * 9.81f;
        
        [Header("Horizontal Movements")]
        public float maxSpeed = 8f;
        public float acceleration = 50f;
        public float maxAccelerationForce = 150f;
        public float deceleration = 50f;
        public float maxDecelerationForce = 150f;
    }
}
