using Core;
using Core.Saving;
using UnityEngine;
using Movement;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;

namespace Combat
{
    public class Fighter : MonoBehaviour, IAction, IJsonSaveable
    {
        [SerializeField] private float timeBetweenAttacks = 2f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] private Weapon defaultWeapon = null;
        
        private Health _target;
        private Mover _mover;
        private ActionScheduler _actionScheduler;
        private Animator _animator;
        
        private float _timeSinceLastAttack = Mathf.Infinity;
        private Weapon _currentWeapon = null;

        private void Start()
        {
            InitializeComponents();
            if (_currentWeapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
        }
        
        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            
            if (_target == null) return;
            if (_target.IsDead) return;
            
            if (!GetIsInRange())
            {
                _mover.MoveTo(_target.transform.position, 1f);
            }
            else
            {
                _mover.Cancel();
                AttackBehaviour();
            }
        }

        public  void EquipWeapon(Weapon weapon)
        {
            _currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        private void InitializeComponents()
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

        private void AttackBehaviour()
        {
            transform.LookAt(_target.transform);
            if (_timeSinceLastAttack > timeBetweenAttacks)
            {
                // This will trigger Hit() event
                TriggerAttackAnimation();
                _timeSinceLastAttack = 0;
            }

        }

        private void TriggerAttackAnimation()
        {
            _animator.ResetTrigger("attack");
            _animator.SetTrigger("attack");
        }

        // Animation Event
         void Hit()
        {
            if (_target == null) return;
            
            if (_currentWeapon.HasProjectile())
            {
                _currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, _target);
            }
            else
            {
                _target.TakeDamage(_currentWeapon.GetDamage());
            }
            
        }
         
         // Animation Event
         void Shoot()
        {
            Hit();
        }

        public void Attack(GameObject combatTarget)
        {
            _actionScheduler.StartAction(this);
            _target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead;
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) <= _currentWeapon.GetRange();
        }

        public void Cancel()
        {
            StopCurrentAttack();
            _target = null;
        }

        private void StopCurrentAttack()
        {
            _animator.ResetTrigger("attack");
            _animator.SetTrigger("stopAttack");
        }
        
        public Weapon GetWeapon()
        {
            return _currentWeapon;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_currentWeapon.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            Weapon weaponLoaded = Resources.Load<Weapon>(state.ToObject<string>());
            EquipWeapon(weaponLoaded);
        }
    }
}