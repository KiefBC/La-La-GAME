using Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    /// <summary>
    /// Handles projectile behavior, including movement, targeting, and damage dealing.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float projectileTravelSpeed = 1f;
        [SerializeField] private bool isHoming = true;
        [SerializeField] private GameObject hitEffect = null;
        [SerializeField] private float maxLifeTime = 10f;
        [SerializeField] private GameObject[] destroyOnHit = null;
        [SerializeField] private float lifeAfterImpact = 2f;
        [SerializeField] private UnityEvent projectileHitEvent;
            
        private Health _target = null;
        private float _damage = 0f;
        private GameObject _instigator = null;
    
        /// <summary>
        /// Initializes the projectile by setting its initial direction towards the target.
        /// </summary>
        private void Start()
        {
            transform.LookAt(GetAimPosition());
        }

        /// <summary>
        /// Updates the projectile's position and direction each frame.
        /// For non-homing projectiles, updates direction only if target is alive.
        /// </summary>
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
        
        /// <summary>
        /// Sets up the projectile with target information and damage values.
        /// </summary>
        /// <param name="target">The target Health component to aim at</param>
        /// <param name="instigator">The GameObject that fired this projectile</param>
        /// <param name="damage">The amount of damage this projectile should deal</param>
        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            if (!target) return; 
            _target = target;
            _damage = damage;
            _instigator = instigator;
            Destroy(gameObject, maxLifeTime);
        }

        /// <summary>
        /// Calculates the aim position, targeting the center of the target's collider if available.
        /// </summary>
        /// <returns>The Vector3 position to aim at</returns>
        private Vector3 GetAimPosition()
        {
            CapsuleCollider targetCollider = _target.GetComponent<CapsuleCollider>();
            if (targetCollider == null) return _target.transform.position;
            return _target.transform.position + targetCollider.center;
        }
        
        /// <summary>
        /// Handles collision with the target, applying damage and triggering effects.
        /// </summary>
        /// <param name="other">The Collider that was hit</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != _target) return;
            if (_target.IsDead) return;
            
            _target.TakeDamage(_instigator, _damage);
            projectileTravelSpeed = 0;
            projectileHitEvent.Invoke();
            
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