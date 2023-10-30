using UnityEngine;

namespace RBCC.Scripts.Environment.Platforms
{
    [RequireComponent(typeof(Rigidbody))]
    public class KinematicRotatingPlatform : MonoBehaviour
    {
        [SerializeField] private float angularSpeed;
        [SerializeField] private Vector3 axis = Vector3.up;

        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Quaternion deltaRotation = Quaternion.Euler(angularSpeed * Time.fixedDeltaTime * axis.normalized);
            _rb.MoveRotation(_rb.rotation * deltaRotation);
        }
    }
}
