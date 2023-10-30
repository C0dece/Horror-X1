using UnityEngine;

namespace RBCC.Scripts.Environment.Platforms
{
    [RequireComponent(typeof(Rigidbody))]
    public class DynamicRotatingPlatform : MonoBehaviour
    {
        [SerializeField] private float torque;
        [SerializeField] private Vector3 axis = Vector3.up;

        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            _rb.AddTorque(torque * axis.normalized);
        }
    }
}
