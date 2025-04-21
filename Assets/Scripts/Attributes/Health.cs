using Core;
using Core.Saving;
using Newtonsoft.Json.Linq;
using Stats;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Attributes
{
    public class Health : MonoBehaviour, IJsonSaveable
    {
        private static readonly int Die1 = Animator.StringToHash("die");
        [SerializeField] private float regenerationPercentage = 70f;
        [SerializeField] private UnityEvent<float> takeDamageEvent;
        [SerializeField] private UnityEvent dieEvent;
        
        private float _healthPoints = -1f; // Cause error if not set
        
        private Animator _animator;
        private bool _isDead = false;
        private CapsuleCollider _capsuleCollider;
        private NavMeshAgent _navMeshAgent;
        private ActionScheduler _scheduler;
        
        public bool IsDead => _isDead;

        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            GetComponent<BaseStats>().OnLevelUp += RegenerateHealth;
            if (_healthPoints < 0)
            {
                _healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
            }
        }

        private void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * regenerationPercentage / 100;
            _healthPoints = Mathf.Max(_healthPoints, regenHealthPoints);
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
        
        public float GetHealthPoints()
        {
            return _healthPoints;
        }
        
        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject instigator,float damage)
        {
            _healthPoints = Mathf.Max(_healthPoints - damage, 0);
            if (_healthPoints <= 0)
            {
                dieEvent.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamageEvent.Invoke(damage);
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience xp = instigator.GetComponent<Experience>();
            if (xp == null) return;
            xp.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void Die()
        {
            if (_isDead) return;
            
            _isDead = true;
            _animator.SetTrigger(Die1);
            _scheduler.CancelCurrentAction();
        }

        private void OnEnable()
        {
            if (_capsuleCollider != null)
            {
                _capsuleCollider.enabled = !_isDead;
            }
            if (_navMeshAgent != null)
            {
                _navMeshAgent.enabled = !_isDead;
            }
        }
        
        private void OnDisable()
        {
            if (_capsuleCollider != null)
            {
                _capsuleCollider.enabled = !_isDead;
            }
            if (_navMeshAgent != null)
            {
                _navMeshAgent.enabled = !_isDead;
            }
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_healthPoints);
        }

        public void RestoreFromJToken(JToken state)
        {
            _healthPoints = state.ToObject<float>();
            Debug.Log("Restored HP: " + _healthPoints);
            if (_healthPoints <= 0)
            {
                Die();
            }
        }

        public void Heal(float healthRestore)
        {
            if (_healthPoints <= 0) return;
            _healthPoints = Mathf.Min(_healthPoints + healthRestore, GetMaxHealthPoints());
        }
    }
}