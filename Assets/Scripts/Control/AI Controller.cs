using Attributes;
using Combat;
using UnityEngine;
using Core;
using Movement;

namespace Control
{
    /// <summary>
    /// Controls AI behavior including patrolling, combat, and state management.
    /// Handles transitions between different AI states and their corresponding behaviors.
    /// </summary>
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 10f;
        [SerializeField] private float suspicionTime = 3f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] float wayPointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 2f;
        
        [Range(0, 1)]
        [SerializeField] private float patrolSpeedFraction = 0.1f;

        private GameObject _player;
        private Fighter _fighter;
        private Health _health;
        private Mover _mover;
        private ActionScheduler _actionScheduler;
        
        private Vector3 _guardLocation;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceArrivedAtWayPoint = Mathf.Infinity;
        private int _currentWayPointIndex = 0;

        private AIState _currentState;

        private enum AIState
        {
            Patrol,
            Suspicious,
            Attack
        }

        /// <summary>
        /// Initializes the AI controller by setting up required component references.
        /// </summary>
        private void Awake()
        {
            InitializeComponents();
        }
        
        /// <summary>
        /// Sets the initial state of the AI to patrol mode.
        /// </summary>
        private void Start()
        {
            _currentState = AIState.Patrol;
        }

        /// <summary>
        /// Initializes and validates all required component references.
        /// Logs errors if any required components are missing.
        /// </summary>
        private void InitializeComponents()
        {
            _player = GameObject.FindWithTag("Player");
            if (_player == null)
            {
                Debug.LogError($"Missing Player object in the scene");
            }
            
            _fighter = GetComponent<Fighter>();
            if (_fighter == null)
            {
                Debug.LogError($"Missing Fighter component on {gameObject.name}");
            }
            
            _health = GetComponent<Health>();
            if (_health == null)
            {
                Debug.LogError($"Missing Health component on {gameObject.name}");
            }
            
            _mover = GetComponent<Mover>();
            if (_mover == null)
            {
                Debug.LogError($"Missing Mover component on {gameObject.name}");
            }
            
            _actionScheduler = GetComponent<ActionScheduler>();
            if (_actionScheduler == null)
            {
                Debug.LogError($"Missing ActionScheduler component on {gameObject.name}");
            }
            
            _guardLocation = transform.position;
        }

        /// <summary>
        /// Updates the AI's behavior each frame, handling state transitions and actions.
        /// </summary>
        private void Update()
        {
            if (_health.IsDead) return;
            
            UpdateState();
            UpdateBehaviour();
            UpdateTimers();
        }
        
        /// <summary>
        /// Generates a random dwell time for waypoint pauses.
        /// </summary>
        /// <returns>A randomized dwell time based on the configured waypointDwellTime</returns>
        private float GetRandomDwellTime()
        {
            return waypointDwellTime + UnityEngine.Random.Range(-waypointDwellTime / 2, waypointDwellTime / 2);
        }

        /// <summary>
        /// Updates the AI's current state based on conditions and transitions.
        /// Handles state changes between Patrol, Suspicious, and Attack states.
        /// </summary>
        private void UpdateState()
        {
            AIState newState = _currentState;

            switch (_currentState)
            {
                case AIState.Patrol:
                    if (InAttackRangeOfPlayer() && _fighter.CanAttack(_player))
                    {
                        newState = AIState.Attack;
                    }
                    break;

                case AIState.Suspicious:
                    if (InAttackRangeOfPlayer() && _fighter.CanAttack(_player))
                    {
                        newState = AIState.Attack;
                    }
                    else if (_timeSinceLastSawPlayer >= suspicionTime)
                    {
                        newState = AIState.Patrol;
                    }
                    break;

                case AIState.Attack:
                    if (!InAttackRangeOfPlayer() || !_fighter.CanAttack(_player))
                    {
                        newState = AIState.Suspicious;
                        _timeSinceLastSawPlayer = 0;
                    }
                    break;
            }

            if (newState != _currentState)
            {
                OnStateExit(_currentState);
                _currentState = newState;
                OnStateEnter(_currentState);
            }
        }

        /// <summary>
        /// Updates the AI's behavior based on its current state.
        /// </summary>
        private void UpdateBehaviour()
        {
            switch (_currentState)
            {
                case AIState.Patrol:
                    PatrolBehaviour();
                    break;
                case AIState.Suspicious:
                    SuspicionBehaviour();
                    break;
                case AIState.Attack:
                    AttackBehaviour();
                    break;
            }
        }

        /// <summary>
        /// Handles state entry actions for each AI state.
        /// </summary>
        /// <param name="state">The state being entered</param>
        private void OnStateEnter(AIState state)
        {
            switch (state)
            {
                case AIState.Patrol:
                    _timeSinceArrivedAtWayPoint = Mathf.Infinity;
                    break;
                case AIState.Suspicious:
                    _mover.Cancel();
                    _actionScheduler.CancelCurrentAction();
                    break;
                case AIState.Attack:
                    _timeSinceLastSawPlayer = 0;
                    break;
            }
        }

        /// <summary>
        /// Handles state exit actions for each AI state.
        /// </summary>
        /// <param name="state">The state being exited</param>
        private void OnStateExit(AIState state)
        {
            switch (state)
            {
                case AIState.Attack:
                    _fighter.Cancel();
                    break;
            }
        }

        /// <summary>
        /// Updates the timers used for state transitions and behavior timing.
        /// </summary>
        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWayPoint += Time.deltaTime;
        }

        /// <summary>
        /// Handles the patrol behavior, including waypoint navigation and guard position returns.
        /// </summary>
        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardLocation;
            if (patrolPath != null)
            {
                if (AtWayPoint())
                {
                    _timeSinceArrivedAtWayPoint = 0;
                    CycleWayPoints();
                }
                nextPosition = GetCurrentWayPoint();
            }
            
            if (_timeSinceArrivedAtWayPoint > GetRandomDwellTime())
            {
                _mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        /// <summary>
        /// Gets the current waypoint position from the patrol path.
        /// </summary>
        /// <returns>The position of the current waypoint</returns>
        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.GetWayPoint(_currentWayPointIndex);
        }

        /// <summary>
        /// Advances to the next waypoint in the patrol path.
        /// </summary>
        private void CycleWayPoints()
        {
            _currentWayPointIndex = patrolPath.GetNextIndex(_currentWayPointIndex);
        }

        /// <summary>
        /// Checks if the AI has reached the current waypoint.
        /// </summary>
        /// <returns>True if within waypoint tolerance, false otherwise</returns>
        private bool AtWayPoint()
        {
            float distanceToWayPoint = Vector3.Distance(transform.position, GetCurrentWayPoint());
            return distanceToWayPoint < wayPointTolerance;
        }

        /// <summary>
        /// Handles the suspicious behavior state, canceling current actions.
        /// </summary>
        private void SuspicionBehaviour()
        {
            _mover.Cancel();
            _actionScheduler.CancelCurrentAction();
        }

        /// <summary>
        /// Handles the attack behavior state, updating player tracking time.
        /// </summary>
        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_player);
        }

        /// <summary>
        /// Checks if the player is within attack range.
        /// </summary>
        /// <returns>True if player is within chase distance, false otherwise</returns>
        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}