using Attributes;
using UnityEngine;
using UnityEngine.AI;
using Core;
using Core.Saving;
using Newtonsoft.Json.Linq;

namespace Movement
{
    /// <summary>
    /// Handles character movement using Unity's NavMeshAgent component.
    /// Implements action scheduling and state saving/loading functionality.
    /// </summary>
    public class Mover : MonoBehaviour, IAction, IJsonSaveable
    {
        private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");
        [SerializeField] private Transform target;
        [SerializeField] private float maxSpeed = 6f; // Maximum speed of the agent
        
        private NavMeshAgent _agent;
        private Animator _animator;
        private ActionScheduler _scheduler;
        private Health _health;

        /// <summary>
        /// Initializes required components on object creation.
        /// </summary>
        private void Awake()
        {
            InitializeComponents();
        }

        /// <summary>
        /// Gets and validates all required components, logging errors if any are missing.
        /// </summary>
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

        /// <summary>
        /// Updates the NavMeshAgent state and animation parameters each frame.
        /// Disables navigation when character is dead.
        /// </summary>
        void Update()
        {
            _agent.enabled = !_health.IsDead;
            
            UpdateAnimator();
        }

        /// <summary>
        /// Moves the character to a specified destination using NavMeshAgent.
        /// </summary>
        /// <param name="destination">Target world position to move to</param>
        /// <param name="speedFraction">Movement speed as a fraction of maximum speed (0-1)</param>
        public void MoveTo(Vector3 destination, float speedFraction)
        {
            _agent.destination = destination;
            _agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            _agent.isStopped = false;
        }

        /// <summary>
        /// Updates the animator's forward speed parameter based on current velocity.
        /// </summary>
        private void UpdateAnimator()
        {
            Vector3 velocity = _agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            _animator.SetFloat(ForwardSpeed, speed);
        }

        /// <summary>
        /// Stops the current movement action.
        /// Implements IAction interface.
        /// </summary>
        public void Cancel()
        {
            _agent.isStopped = true;
        }
        
        /// <summary>
        /// Initiates a new movement action with specified parameters.
        /// </summary>
        /// <param name="destination">Target world position to move to</param>
        /// <param name="speedFraction">Movement speed as a fraction of maximum speed (0-1)</param>
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            _scheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        /// <summary>
        /// Captures the current position for saving.
        /// Implements IJsonSaveable interface.
        /// </summary>
        /// <returns>JToken containing serialized position data</returns>
        public JToken CaptureAsJToken()
        {
            return transform.position.ToToken();
        }

        /// <summary>
        /// Restores the position from saved data.
        /// Temporarily disables NavMeshAgent during position update to prevent conflicts.
        /// Implements IJsonSaveable interface.
        /// </summary>
        /// <param name="state">JToken containing serialized position data</param>
        public void RestoreFromJToken(JToken state)
        {
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = state.ToVector3();
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}