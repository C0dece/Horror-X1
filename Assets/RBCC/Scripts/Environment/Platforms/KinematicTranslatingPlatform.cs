using UnityEngine;

namespace RBCC.Scripts.Environment.Platforms
{
    /// <summary>
    /// Continuous translating platform that moves with a rigidbody.
    /// This is a simple example of how it can be implemented with the player.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class KinematicTranslatingPlatform : MonoBehaviour
    {
        [SerializeField] private Vector3 startPoint;
        [SerializeField] private Vector3 endPoint;
        [SerializeField] private float speed;

        private Rigidbody _rb;
    
        private const float Offset = 0.3f;
        private float _dir = 1f;
    
        private Vector3 _startPoint;
        private Vector3 _endPoint;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _startPoint = transform.TransformPoint(startPoint);
            _endPoint = transform.TransformPoint(endPoint);
        }

        private void FixedUpdate()
        {
            if (Vector3.Distance(transform.position, _dir > 0f ? _endPoint : _startPoint) < Offset) 
                _dir = -_dir;
        
            Vector3 direction = _dir * (_endPoint - _startPoint).normalized;
        
            _rb.MovePosition(_rb.position + speed * Time.fixedDeltaTime * direction);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.TransformPoint(Application.isPlaying ? _startPoint : startPoint), 0.25f);
            Gizmos.DrawSphere(transform.TransformPoint(Application.isPlaying ? _endPoint : endPoint), 0.25f);
        }
    }
}
