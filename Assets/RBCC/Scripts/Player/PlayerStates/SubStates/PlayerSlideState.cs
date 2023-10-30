using RBCC.Scripts.Player.PlayerStateData;
using RBCC.Scripts.Utils;
using UnityEngine;

namespace RBCC.Scripts.Player.PlayerStates.SubStates
{
    public class PlayerSlideState : PlayerBaseState
    {
        private readonly PlayerSlideData slideData;

        private float _currentSlopeAngle;
        private float _currentSlideForce; // current slide force applied to the character. Feel free to modify its value as you see fit.
        private float _currentTorqueForce; // current torque applied to the character.
        
        #region Overrides

        public PlayerSlideState(PlayerStateController ctx, PlayerStateFactory factory, PlayerSlideData slideData) : base(ctx, factory)
        {
            this.slideData = slideData;
        }

        public override PlayerState State => PlayerState.PlayerSlideState;

        public override void EnterState()
        {
            // Init current slope angle.
            _currentSlopeAngle = Vector3.Angle(Ctx.groundRayDir, -Ctx.GroundHit.normal);
            
            // Set up movement speed
            Ctx.maxSpeed = slideData.maxSpeed;
            Ctx.acceleration = slideData.acceleration;
            Ctx.maxAccelerationForce = slideData.maxAccelerationForce;
            Ctx.deceleration = slideData.deceleration;
            Ctx.maxDecelerationForce = slideData.maxDecelerationForce;
            
            // Set up slide force
            _currentSlideForce = 0f;
            _currentTorqueForce = 0f;
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void FixedUpdateState()
        {
            // Increase the slide force over time with an acceleration
            _currentSlideForce = Mathf.Clamp(_currentSlideForce + slideData.slideForceAcc * Time.fixedDeltaTime, 0f,
                slideData.slideForce);
            // Increase the slide force over time with an acceleration
            _currentTorqueForce = Mathf.Clamp(_currentTorqueForce + slideData.slideTorqueAcc * Time.fixedDeltaTime, 0f,
                slideData.slideTorque);
            
            Slide();
        }

        public override void ExitState()
        {
        }

        public override void CheckSwitchStates()
        {
            // From one slope to another
            float slopeAngle = Vector3.Angle(Ctx.groundRayDir, -Ctx.GroundHit.normal);
            if (!MathUtils.EqualApprox(_currentSlopeAngle, slopeAngle, 1f) && Ctx.IsSliding)
            {
                SwitchState(Factory.Slide());
                return;
            }

            if (!Ctx.IsSliding)
            {
                // To Idle
                if (!Ctx.HorizontalInputsDetected && !Ctx.IsSliding)
                {
                    SwitchState(Factory.Idle());
                    return;
                }
            
                // To Walk
                if (Ctx.HorizontalInputsDetected && !Ctx.RunKeyPerformed && !Ctx.IsSliding)
                {
                    SwitchState(Factory.Walk());
                    return;
                }
            
                // To Run
                if (Ctx.HorizontalInputsDetected && Ctx.RunKeyPerformed && !Ctx.IsSliding)
                {
                    SwitchState(Factory.Run());
                    return;
                }
            }
        }

        public override void InitializeSubState()
        {
        }

        #endregion

        #region Methods

        private void Slide()
        {
            // Calculate current slope vectors.
            Vector3 slopeTransversal = Vector3.Cross(-Ctx.GroundHit.normal, Ctx.groundRayDir); // Direction to the side of the slope
            Vector3 slopeDir = Quaternion.AngleAxis(90f, slopeTransversal) * -Ctx.GroundHit.normal; // Direction to the bottom parallel to the slope
            Vector3 horizontalSlopeDir = Vector3.Cross(Ctx.UpDirection, slopeTransversal); // Normal direction

            // Apply torque to rotate the player towards the bottom of the slope
            Quaternion desiredRotation = Quaternion.LookRotation(horizontalSlopeDir);
            Quaternion toGoal = MathUtils.ShortestRotation(Ctx.transform.rotation, desiredRotation);
            toGoal.ToAngleAxis(out var rotDegrees, out var rotAxis);
            rotAxis.Normalize();
            float rotRadians = rotDegrees * Mathf.Deg2Rad;
            Ctx.Rb.AddTorque((rotAxis * (rotRadians * _currentTorqueForce)), ForceMode.Acceleration);
            
            // Slope direction towards the ground
            Vector3 forceDir = Quaternion.AngleAxis(90f, slopeTransversal) * -Ctx.GroundHit.normal;
            // Debug.DrawRay(transform.position, forceDir.normalized * 3f);
        
            // Note: you can adjust the slide force here depending on the ground you are on.
            // For example: by default the slide force is increased with the slope angle (with the following Mathf.Sin)

            // Apply slideForce
            float slopeAngle = Vector3.Angle(Ctx.groundRayDir, -Ctx.GroundHit.normal);
            Vector3 force = _currentSlideForce * (1f + Mathf.Sin(slopeAngle * Mathf.Deg2Rad)) *
                            Ctx.Rb.mass * Physics.gravity.magnitude * forceDir;

            Ctx.Rb.AddForce(force);
        }

        #endregion
    }
}
