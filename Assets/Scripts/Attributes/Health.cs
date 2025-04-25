using Core;
using Core.Saving;
using Newtonsoft.Json.Linq;
using Stats;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Attributes
{
    /// <summary>
    /// Manages entity health, including damage handling, death state, health regeneration,
    /// and save/load functionality. Supports both player and non-player entities.
    /// </summary>
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
        private GameOverController _gameOverController;

        /// <summary>
        /// Gets whether the entity is currently dead.
        /// </summary>
        public bool IsDead => _isDead;

        /// <summary>
        /// Initializes required components and sets up the game over controller for player entities.
        /// </summary>
        private void Awake()
        {
            InitializeComponents();
            if (CompareTag("Player"))
            {
                _gameOverController = FindAnyObjectByType<GameOverController>();
            }
        }

        /// <summary>
        /// Sets up level-up event handling and initializes health points if not already set.
        /// </summary>
        private void Start()
        {
            GetComponent<BaseStats>().OnLevelUp += RegenerateHealth;
            if (_healthPoints < 0)
            {
                _healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
            }
        }

        /// <summary>
        /// Regenerates health based on the entity's maximum health and regeneration percentage.
        /// Called when the entity levels up.
        /// </summary>
        private void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * regenerationPercentage / 100;
            _healthPoints = Mathf.Max(_healthPoints, regenHealthPoints);
        }

        /// <summary>
        /// Initializes and validates required components for health functionality.
        /// Logs errors if required components are missing.
        /// </summary>
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
        
        /// <summary>
        /// Returns the current health points of the entity.
        /// </summary>
        /// <returns>Current health points</returns>
        public float GetHealthPoints()
        {
            return _healthPoints;
        }
        
        /// <summary>
        /// Returns the maximum possible health points for the entity.
        /// </summary>
        /// <returns>Maximum health points based on stats</returns>
        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        /// <summary>
        /// Handles damage taken by the entity, including death processing and experience rewards.
        /// </summary>
        /// <param name="instigator">The GameObject that caused the damage</param>
        /// <param name="damage">Amount of damage to apply</param>
        public void TakeDamage(GameObject instigator, float damage)
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

        /// <summary>
        /// Awards experience to the instigator when this entity dies.
        /// </summary>
        /// <param name="instigator">The GameObject to receive experience</param>
        private void AwardExperience(GameObject instigator)
        {
            Experience xp = instigator.GetComponent<Experience>();
            if (xp == null) return;
            xp.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        /// <summary>
        /// Processes entity death, including animation triggers and game over handling for player entities.
        /// </summary>
        private void Die()
        {
            if (_isDead) return;
            
            _isDead = true;
            _animator.SetTrigger(Die1);
            _scheduler.CancelCurrentAction();
            
            if (CompareTag("Player") && _gameOverController != null)
            {
                _gameOverController.ShowGameOver();
            }
        }

        /// <summary>
        /// Handles component enabling based on death state.
        /// </summary>
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
        
        /// <summary>
        /// Handles component disabling based on death state.
        /// </summary>
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

        /// <summary>
        /// Implements IJsonSaveable to capture current health state.
        /// </summary>
        /// <returns>JToken containing health points</returns>
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_healthPoints);
        }

        /// <summary>
        /// Implements IJsonSaveable to restore health state from saved data.
        /// </summary>
        /// <param name="state">JToken containing saved health points</param>
        public void RestoreFromJToken(JToken state)
        {
            _healthPoints = state.ToObject<float>();
            Debug.Log("Restored HP: " + _healthPoints);
            if (_healthPoints <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Heals the entity by the specified amount, up to maximum health.
        /// Only works if the entity is not dead.
        /// </summary>
        /// <param name="healthRestore">Amount of health to restore</param>
        public void Heal(float healthRestore)
        {
            if (_healthPoints <= 0) return;
            _healthPoints = Mathf.Min(_healthPoints + healthRestore, GetMaxHealthPoints());
        }
    }
}