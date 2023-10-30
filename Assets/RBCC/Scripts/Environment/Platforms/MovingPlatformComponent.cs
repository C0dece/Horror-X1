using UnityEngine;

namespace RBCC.Scripts.Environment.Platforms
{
    public class MovingPlatformComponent : MonoBehaviour
    {
        private Vector3 _lastPosition;
        private Vector3 _velocity;

        public Vector3 Velocity => _velocity;

        private void Start()
        {
            _lastPosition = transform.position;
        }

        private void FixedUpdate()
        {
            Vector3 currentPos = transform.position;
            _velocity = (currentPos - _lastPosition) / Time.fixedDeltaTime;
            _lastPosition = currentPos;
        }
    }
}
