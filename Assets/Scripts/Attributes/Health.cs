using Core;
using Core.Saving;
using Newtonsoft.Json.Linq;
using Stats;
using UnityEngine;
using UnityEngine.AI;

namespace Attributes
{
    public class Health : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] float healthPoints = 20f;
        private Animator _animator;
        private bool _isDead = false;
        private CapsuleCollider _capsuleCollider;
        private NavMeshAgent _navMeshAgent;
        private ActionScheduler _scheduler;
        
        public bool IsDead => _isDead;
        
        private void Start()
        {
            InitializeComponents();
            healthPoints = GetComponent<BaseStats>().GetHealth();
        }

        private void InitializeComponents()
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
            
            _scheduler = GetComponent<ActionScheduler>();
            if (_scheduler == null)
            {
                Debug.LogError($"Missing ActionScheduler component on {gameObject.name}");
            }
        }

        public void TakeDamage(GameObject instigator,float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            if (healthPoints <= 0)
            {
                Die();
                AwardExperience(instigator);
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience xp = instigator.GetComponent<Experience>();
            if (xp == null) return;
            xp.GainExperience(GetComponent<BaseStats>().GetExperienceReward());
        }

        public float GetHealthPercentage()
        {
            return 100 * (healthPoints / GetComponent<BaseStats>().GetHealth());
        }

        private void Die()
        {
            if (_isDead) return;
            
            _isDead = true;
            _animator.SetTrigger("die");
            _scheduler.CancelCurrentAction();
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(healthPoints);
        }

        public void RestoreFromJToken(JToken state)
        {
            healthPoints = state.ToObject<float>();
            if (healthPoints <= 0)
            {
                Die();
            }
        }
    }
}