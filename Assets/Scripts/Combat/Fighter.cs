using Core;
using UnityEngine;
using Movement;
using Unity.VisualScripting;

namespace Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float timeBetweenAttacks = 2f;
        [SerializeField] private float punchDamage = 10f;
        [SerializeField] private float turnSpeed = 50f; // Add turn speed control
        
        private Transform _target;
        private Mover _mover;
        private ActionScheduler _actionScheduler;
        private Animator _animator;
        
        private float _timeSinceLastAttack = 0;

        private void Start()
        {
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
            
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError($"Missing Animator component on {gameObject.name}");
            }
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            
            if (_target == null) return;
            
            if (!GetIsInRange())
            {
                _mover.MoveTo(_target.position);
            }
            else
            {
                _mover.Cancel();
                FaceTarget();
                AttackBehaviour();
            }
        }

        private void FaceTarget()
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }

        private void AttackBehaviour()
        {
            if (_timeSinceLastAttack > timeBetweenAttacks)
            {
                // This will trigger Hit() event
                _animator.SetTrigger("attack");
                _timeSinceLastAttack = 0;
            }

        }
        
        // Animation Event
         void Hit()
        {
            Health targetHealth = _target.GetComponent<Health>();
            targetHealth.TakeDamage(punchDamage);
        }

        public void Attack(CombatTarget combatTarget)
        {
            _actionScheduler.StartAction(this);
            _target = combatTarget.transform;
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.position) <= weaponRange;
        }

        public void Cancel()
        {
            _target = null;
        }
    }
}