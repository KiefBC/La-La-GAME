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
    /// <summary>
    /// Manages combat behavior including weapon handling, attacking, and damage calculations.
    /// Implements action scheduling, saving/loading, and stat modification systems.
    /// </summary>
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

        /// <summary>
        /// Initializes components and sets up the default weapon configuration.
        /// </summary>
        private void Awake()
        {
            InitializeComponents();
            _currentWeaponConfig = defaultWeaponConfig;
            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        /// <summary>
        /// Forces initialization of the current weapon.
        /// </summary>
        private void Start()
        {
            _currentWeapon.ForceInit();
        }

        /// <summary>
        /// Handles attack timing and movement towards target.
        /// </summary>
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
        
        /// <summary>
        /// Equips a new weapon and updates the current weapon configuration.
        /// </summary>
        /// <param name="weaponConfig">The weapon configuration to equip</param>
        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            _currentWeaponConfig = weaponConfig;
            _currentWeapon.value = AttachWeapon(weaponConfig);
        }
        
        /// <summary>
        /// Attaches a weapon to the character's hand transforms and sets up animations.
        /// </summary>
        /// <param name="weaponConfig">The weapon configuration to attach</param>
        /// <returns>The instantiated weapon</returns>
        private Weapon AttachWeapon(WeaponConfig weaponConfig)
        {
            Animator animator = GetComponent<Animator>();
            return weaponConfig.Spawn(rightHandTransform, leftHandTransform, animator);
        }
        
        /// <summary>
        /// Returns the current combat target.
        /// </summary>
        /// <returns>The current target's Health component</returns>
        public Health GetTarget()
        {
            return _target;
        }
        
        /// <summary>
        /// Handles the attack behavior including facing the target and timing attacks.
        /// </summary>
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
        
        /// <summary>
        /// Triggers the attack animation.
        /// </summary>
        private void TriggerAttackAnimation()
        {
            _animator.ResetTrigger(Attack1);
            _animator.SetTrigger(Attack1);
        }

        /// <summary>
        /// Sets up and returns the default weapon.
        /// </summary>
        /// <returns>The default weapon instance</returns>
        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeaponConfig);
        }
        
        /// <summary>
        /// Animation event handler for hit detection.
        /// Handles both melee and projectile weapon damage.
        /// </summary>
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
        
        /// <summary>
        /// Animation event handler for ranged attacks.
        /// </summary>
        void Shoot()
        {
            Hit();
        }
        
        /// <summary>
        /// Checks if the target is within the weapon's range.
        /// </summary>
        /// <returns>True if target is in range</returns>
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) <= _currentWeaponConfig.GetRange();
        }
        
        /// <summary>
        /// Checks if a target can be attacked.
        /// </summary>
        /// <param name="combatTarget">The potential target</param>
        /// <returns>True if the target can be attacked</returns>
        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead;
        }
        
        /// <summary>
        /// Initiates an attack on a target.
        /// </summary>
        /// <param name="combatTarget">The target to attack</param>
        public void Attack(GameObject combatTarget)
        {
            _actionScheduler.StartAction(this);
            _target = combatTarget.GetComponent<Health>();
        }
        
        /// <summary>
        /// Cancels the current attack action.
        /// </summary>
        public void Cancel()
        {
            StopCurrentAttack();
            _target = null;
        }
        
        /// <summary>
        /// Stops the current attack animation.
        /// </summary>
        private void StopCurrentAttack()
        {
            _animator.ResetTrigger("attack");
            _animator.SetTrigger("stopAttack");
        }
        
        /// <summary>
        /// Implements IModifierProvider to provide additive damage modifiers.
        /// </summary>
        /// <param name="stat">The stat to modify</param>
        /// <returns>Enumerable of additive modifiers</returns>
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.GetDamage();
            }
        }

        /// <summary>
        /// Implements IModifierProvider to provide percentage damage modifiers.
        /// </summary>
        /// <param name="stat">The stat to modify</param>
        /// <returns>Enumerable of percentage modifiers</returns>
        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.GetPercentDamageBonus();
            }
        }

        /// <summary>
        /// Initializes and validates required components.
        /// </summary>
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
        
        /// <summary>
        /// Returns the current weapon configuration.
        /// </summary>
        /// <returns>The current WeaponConfig</returns>
        public WeaponConfig GetWeapon()
        {
            return _currentWeaponConfig;
        }

        /// <summary>
        /// Implements IJsonSaveable to save the current weapon state.
        /// </summary>
        /// <returns>JToken containing the weapon name</returns>
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_currentWeaponConfig.name);
        }

        /// <summary>
        /// Implements IJsonSaveable to restore the weapon state.
        /// </summary>
        /// <param name="state">JToken containing the weapon name</param>
        public void RestoreFromJToken(JToken state)
        {
            WeaponConfig weaponConfigLoaded = Resources.Load<WeaponConfig>(state.ToObject<string>());
            EquipWeapon(weaponConfigLoaded);
        }
    }
}