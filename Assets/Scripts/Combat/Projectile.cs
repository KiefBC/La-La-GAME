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
            if (!_target) return;
            transform.LookAt(GetAimPosition());
            transform.Translate(Vector3.forward * (projectileTravelSpeed * Time.deltaTime));
        }
        
        public void SetTarget(Health target, float damage)
        {
            if (!target) return;
            Debug.Log("DEBUG :: Projectile target set");
            _target = target; // ??? TODO: See if <this> works
            _damage = damage;
        }

        private Vector3 GetAimPosition()
        {
            CapsuleCollider targetCollider = _target.GetComponent<CapsuleCollider>();
            if (!targetCollider) return _target.transform.position; // If no collider, just aim at the target
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
