using System;
using Combat;
using UnityEngine;
using Core;
using Movement;
using UnityEngine.Serialization;

namespace Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 10f;
        [SerializeField] private float suspicionTime = 3f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] float wayPointTolerance = 1f; // Distance to waypoint
        [SerializeField] float waypointDwellTime = 3f; // Time to wait at each waypoint
        
        private GameObject _player;
        private Fighter _fighter;
        private Health _health;
        private Mover _mover;
        private ActionScheduler _actionScheduler;
        
        private Vector3 _guardLocation; // Home location
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceArrivedAtWayPoint = Mathf.Infinity;
        private int _currentWayPointIndex = 0;

        private void Awake()
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
            
            if (InAttackRangeOfPlayer() && _fighter.CanAttack(_player))
            {
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            
            UpdateTimers();
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
            
            if (_timeSinceArrivedAtWayPoint > waypointDwellTime)
            {
                _mover.StartMoveAction(nextPosition);
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
