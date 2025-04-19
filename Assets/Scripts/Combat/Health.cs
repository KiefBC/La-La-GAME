using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float healthPoints = 20f;
        private Animator _animator;
        private bool _isDead = false;
        private CapsuleCollider _capsuleCollider;
        private NavMeshAgent _navMeshAgent;
        
        public bool IsDead => _isDead;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError($"Missing Animator component on {gameObject.name}");
            }
            
            _capsuleCollider = GetComponent<CapsuleCollider>();
            if (_capsuleCollider == null && !gameObject.CompareTag("Player"))
            {
                Debug.LogError($"Missing CapsuleCollider component on {gameObject.name}");
            }

            _navMeshAgent = GetComponent<NavMeshAgent>();
            if (_navMeshAgent == null)
            {
                Debug.LogError($"Missing NavMeshAgent component on {gameObject.name}");
            }
        }
        
        public void TakeDamage(float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            if (healthPoints <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (_isDead) return;
            
            _isDead = true;
            _animator.SetTrigger("die");
            _capsuleCollider.enabled = false;
            _navMeshAgent.enabled = false;
        }
    }
}