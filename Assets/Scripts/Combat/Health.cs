using UnityEngine;
using UnityEngine.Serialization;

namespace Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float healthPoints = 20f;
        private Animator _animator;
        private bool isDead = false;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError($"Missing Animator component on {gameObject.name}");
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
        
        public bool IsDead => isDead;

        private void Die()
        {
            if (isDead) return;
            
            isDead = true;
            _animator.SetTrigger("die");
        }
    }
}
