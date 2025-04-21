using Attributes;
using UnityEngine;
using UnityEngine.AI;
using Core;
using Core.Saving;
using Newtonsoft.Json.Linq;

namespace Movement
{
    public class Mover : MonoBehaviour, IAction, IJsonSaveable
    {
        private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");
        [SerializeField] private Transform target;
        [SerializeField] private float maxSpeed = 6f; // Maximum speed of the agent
        
        private NavMeshAgent _agent;
        private Animator _animator;
        private ActionScheduler _scheduler;
        private Health _health;

        private void Awake()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent == null)
            {
                Debug.LogError($"Missing NavMeshAgent component on {gameObject.name}");
            }

            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError($"Missing Animator component on {gameObject.name}");
            }
            
            _scheduler = GetComponent<ActionScheduler>();
            if (_scheduler == null)
            {
                Debug.LogError($"Missing ActionScheduler component on {gameObject.name}");
            }
            
            _health = GetComponent<Health>();
            if (_health == null)
            {
                Debug.LogError($"Missing Health component on {gameObject.name}");
            }
        }

        void Update()
        {
            _agent.enabled = !_health.IsDead;
            
            UpdateAnimator();
        }

        /// <summary>
        /// Moves the object to the specified world position using the NavMeshAgent component.
        /// </summary>
        /// <param name="destination">The target position to move the object to.</param>
        /// <param name="speedFraction"></param>
        public void MoveTo(Vector3 destination, float speedFraction)
        {
            _agent.destination = destination;
            _agent.speed = maxSpeed * Mathf.Clamp01(speedFraction); // Clamp the speed to the range [0, 1]
            _agent.isStopped = false;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = _agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            _animator.SetFloat(ForwardSpeed, speed);
        }

        public void Cancel()
        {
            _agent.isStopped = true;
        }
        
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            _scheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public JToken CaptureAsJToken()
        {
            return transform.position.ToToken();
        }

        public void RestoreFromJToken(JToken state)
        {
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = state.ToVector3();
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
