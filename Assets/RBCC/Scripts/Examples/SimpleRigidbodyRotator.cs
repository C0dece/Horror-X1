using UnityEngine;

namespace RBCC.Scripts.Examples
{
    [RequireComponent(typeof(Rigidbody))]
    public class SimpleRigidbodyRotator : MonoBehaviour
    {
        public Vector3 minAngles;
        public Vector3 maxAngles;

        public float angularSpeed;

        private Vector3 _target;
        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        
            _target = maxAngles;
            if (Vector3.Distance(transform.rotation.eulerAngles, maxAngles) < 1f)
            {
                _target = minAngles;
            }
        }

        private void FixedUpdate()
        {
        
            if (Quaternion.Angle(transform.rotation, Quaternion.Euler(minAngles)) < 1f)
            {
                _target = maxAngles;
            }
            if (Quaternion.Angle(transform.rotation, Quaternion.Euler(maxAngles)) < 1f)
            {
                _target = minAngles;
            }

            // transform.rotation =
            //     Quaternion.Lerp(transform.rotation, Quaternion.Euler(target), Time.fixedDeltaTime * angularSpeed);
        
            // transform.rotation = 
            //     Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(target), Time.fixedDeltaTime * angularSpeed);
        
            _rb.MoveRotation(
                Quaternion.RotateTowards(
                    transform.rotation, 
                    Quaternion.Euler(_target),
                    Time.fixedDeltaTime * angularSpeed)
            );
        }
    }
}
