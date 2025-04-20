using Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float projectileTravelSpeed = 1f;
    
        private Health _target = null;
        private float _damage = 0f;
    
    
        void Update()
        {
            if (_target == null) return;
            transform.LookAt(GetAimPosition());
            transform.Translate(Vector3.forward * (projectileTravelSpeed * Time.deltaTime));
        }
        
        public void SetTarget(Health target, float damage)
        {
            if (!target) return;
            _target = target;
            _damage = damage;
        }

        private Vector3 GetAimPosition()
        {
            CapsuleCollider targetCollider = _target.GetComponent<CapsuleCollider>();
            if (targetCollider == null) return _target.transform.position; // If no collider, just aim at the target
            return _target.transform.position + targetCollider.center;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != _target) return;
            _target.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}
