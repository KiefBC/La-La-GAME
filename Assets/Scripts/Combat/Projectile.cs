using Attributes;
using UnityEngine;

namespace Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float projectileTravelSpeed = 1f;
        [SerializeField] private bool isHoming = true;
        [SerializeField] private GameObject hitEffect = null;
        [SerializeField] private float maxLifeTime = 10f;
        [SerializeField] private GameObject[] destroyOnHit = null;
        [SerializeField] private float lifeAfterImpact = 2f;
            
        private Health _target = null;
        private float _damage = 0f;
        private GameObject _instigator = null;
    
    
        private void Start()
        {
            transform.LookAt(GetAimPosition());
        }

        void Update()
        {
            if (_target == null) return;

            if (!isHoming)
            {
                if (!_target.IsDead)
                {
                    transform.LookAt(GetAimPosition());
                }
            }

            transform.Translate(Vector3.forward * (projectileTravelSpeed * Time.deltaTime));
        }
        
        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            if (!target) return; 
            _target = target;
            _damage = damage;
            _instigator = instigator;
            Destroy(gameObject, maxLifeTime);
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
            if (_target.IsDead) return;
            _target.TakeDamage(_instigator, _damage);

            projectileTravelSpeed = 0;
            
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            
            foreach (GameObject obj in destroyOnHit)
            {
                Destroy(obj);
            }
            
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
