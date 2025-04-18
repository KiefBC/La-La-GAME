using UnityEngine;
using UnityEngine.AI;
using Core;

namespace Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");
        [SerializeField] private Transform target;
        
        private NavMeshAgent _agent;
        private Animator _animator;
        private ActionScheduler _scheduler;

        private void Start()
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
        }

        void Update()
        {
            UpdateAnimator();
        }

        /// <summary>
        /// Moves the object to the specified world position using the NavMeshAgent component.
        /// </summary>
        /// <param name="destination">The target position to move the object to.</param>
        public void MoveTo(Vector3 destination)
        {
            _agent.destination = destination;
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
        
        public void StartMoveAction(Vector3 destination)
        {
            _scheduler.StartAction(this);
            MoveTo(destination);
        }
    }
}
