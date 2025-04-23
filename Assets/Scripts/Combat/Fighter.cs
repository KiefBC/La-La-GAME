using System;
using System.Collections.Generic;
using Attributes;
using Core;
using Core.Saving;
using UnityEngine;
using Movement;
using Newtonsoft.Json.Linq;
using Stats;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Combat
{
    public class Fighter : MonoBehaviour, IAction, IJsonSaveable, IModifierProvider
    {
        private static readonly int Attack1 = Animator.StringToHash("attack");
        [SerializeField] private float timeBetweenAttacks = 2f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] private WeaponConfig defaultWeaponConfig = null;
        [SerializeField] private UnityEvent meleeHitEvent;
        [SerializeField] private GameObject hitEffect = null;

        private Health _target;
        private Mover _mover;
        private ActionScheduler _actionScheduler;
        private Animator _animator;
        
        private float _timeSinceLastAttack = Mathf.Infinity;
        private WeaponConfig _currentWeaponConfig = null;
        LazyValue<Weapon> _currentWeapon;

        private void Awake()
        {
            InitializeComponents();
            _currentWeaponConfig = defaultWeaponConfig;
            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private void Start()
        {
            _currentWeapon.ForceInit();
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
        
        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            _currentWeaponConfig = weaponConfig;
            _currentWeapon.value = AttachWeapon(weaponConfig);
        }
        
        private Weapon AttachWeapon(WeaponConfig weaponConfig)
        {
            Animator animator = GetComponent<Animator>();
            return weaponConfig.Spawn(rightHandTransform, leftHandTransform, animator);
        }
        
        public Health GetTarget()
        {
            return _target;
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
            _animator.ResetTrigger(Attack1);
            _animator.SetTrigger(Attack1);
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeaponConfig);
        }
        
        // Animation Event
        void Hit()
        {
            if (_target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (_currentWeapon.value != null)
            {
                _currentWeapon.value.OnHit();
            }
            
            if (_currentWeaponConfig.HasProjectile())
            {
                _currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, _target, gameObject, damage);
            }
            else
            {
                meleeHitEvent.Invoke();
                _target.TakeDamage(gameObject, damage);
                
                // Spawn hit effect if it exists
                if (hitEffect != null)
                {
                    Vector3 hitPosition = _target.transform.position;
                    CapsuleCollider targetCollider = _target.GetComponent<CapsuleCollider>();
                    if (targetCollider != null)
                    {
                        hitPosition += targetCollider.center;
                    }
                    Instantiate(hitEffect, hitPosition, Quaternion.identity);
                }
            }
        }
        
        // Animation Event
        void Shoot()
        {
            Hit();
        }
        
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) <= _currentWeaponConfig.GetRange();
        }
        
        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead;
        }
        
        public void Attack(GameObject combatTarget)
        {
            _actionScheduler.StartAction(this);
            _target = combatTarget.GetComponent<Health>();
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
        
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.GetPercentDamageBonus();
            }
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
        
        public WeaponConfig GetWeapon()
        {
            return _currentWeaponConfig;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_currentWeaponConfig.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            WeaponConfig weaponConfigLoaded = Resources.Load<WeaponConfig>(state.ToObject<string>());
            EquipWeapon(weaponConfigLoaded);
        }
    }
}