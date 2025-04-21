using Attributes;
using Combat;
using UnityEngine;
using Core;
using Movement;

namespace Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 10f;
        [SerializeField] private float suspicionTime = 3f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] float wayPointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 2f;
        
        [Range(0, 1)]
        [SerializeField] private float patrolSpeedFraction = 0.1f; // Fraction of max walking speed to use when patrolling (30% of 1)

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

        private void Awake()
        {
            InitializeComponents();
        }
        
        private void Start()
        {
            _currentState = AIState.Patrol;
        }

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

        private void Update()
        {
            if (_health.IsDead) return;
            
            UpdateState();
            UpdateBehaviour();
            UpdateTimers();
        }
        
        private float GetRandomDwellTime()
        {
            return waypointDwellTime + UnityEngine.Random.Range(-waypointDwellTime / 2, waypointDwellTime / 2);
        }

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

        private void OnStateEnter(AIState state)
        {
            switch (state)
            {
                case AIState.Patrol:
                    _timeSinceArrivedAtWayPoint = Mathf.Infinity; // Force immediate movement
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

        private void OnStateExit(AIState state)
        {
            switch (state)
            {
                case AIState.Attack:
                    _fighter.Cancel();
                    break;
            }
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWayPoint += Time.deltaTime;
        }

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

        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.GetWayPoint(_currentWayPointIndex);
        }

        private void CycleWayPoints()
        {
            _currentWayPointIndex = patrolPath.GetNextIndex(_currentWayPointIndex);
        }

        private bool AtWayPoint()
        {
            float distanceToWayPoint = Vector3.Distance(transform.position, GetCurrentWayPoint());
            return distanceToWayPoint < wayPointTolerance;
        }

        private void SuspicionBehaviour()
        {
            _mover.Cancel();
            _actionScheduler.CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_player);
        }

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
