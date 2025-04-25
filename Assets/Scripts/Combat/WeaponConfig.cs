using Attributes;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Scriptable Object that defines the configuration for a weapon, including its animations,
    /// damage values, range, and projectile capabilities.
    /// </summary>
    [CreateAssetMenu(fileName = "Weapon", menuName = "The Game/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [Header("Animation")]
        [SerializeField] AnimatorOverrideController playerAnimatorOverride = null;
        [SerializeField] AnimatorOverrideController enemyAnimatorOverride = null;
        
        [Header("Weapon Settings")]
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] private float weaponDamage = 5f;
        [SerializeField] private float percentDamageBonus = 0f;
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile = null;

        private const string WeaponName = "Weapon";
        
        /// <summary>
        /// Spawns the weapon in the appropriate hand and sets up its animations.
        /// </summary>
        /// <param name="rightHand">Transform for the right hand attachment point</param>
        /// <param name="leftHand">Transform for the left hand attachment point</param>
        /// <param name="animator">Animator component to override with weapon-specific animations</param>
        /// <returns>The spawned weapon instance, or null if no prefab is set</returns>
        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;
            
            if (equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = WeaponName;
            }

            if (animator != null)
            {
                AnimatorOverrideController overrideToUse = GetAnimatorOverride(animator.gameObject);
                if (overrideToUse != null)
                {
                    animator.runtimeAnimatorController = overrideToUse;
                }
            }

            return weapon;
        }

        /// <summary>
        /// Gets the appropriate animator override controller based on whether the owner is a player or enemy.
        /// </summary>
        /// <param name="owner">The GameObject that owns this weapon</param>
        /// <returns>The appropriate animator override controller for the owner type</returns>
        private AnimatorOverrideController GetAnimatorOverride(GameObject owner)
        {
            return owner.CompareTag("Player") ? playerAnimatorOverride : enemyAnimatorOverride;
        }

        /// <summary>
        /// Destroys any existing weapon attached to either hand.
        /// </summary>
        /// <param name="rightHand">Transform for the right hand to check</param>
        /// <param name="leftHand">Transform for the left hand to check</param>
        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = GetOldWeapon(rightHand, leftHand);
            if (oldWeapon == null) return;
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        /// <summary>
        /// Finds any existing weapon attached to either hand.
        /// </summary>
        /// <param name="rightHand">Transform for the right hand to check</param>
        /// <param name="leftHand">Transform for the left hand to check</param>
        /// <returns>Transform of the found weapon, or null if none exists</returns>
        private Transform GetOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(WeaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(WeaponName);
            }
            return oldWeapon;
        }

        /// <summary>
        /// Determines which hand transform to use based on the isRightHanded setting.
        /// </summary>
        /// <param name="rightHand">Transform for the right hand</param>
        /// <param name="leftHand">Transform for the left hand</param>
        /// <returns>The appropriate hand transform for weapon attachment</returns>
        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform = isRightHanded ? rightHand : leftHand;
            return handTransform;
        }

        /// <summary>
        /// Creates and launches a projectile from the weapon.
        /// </summary>
        /// <param name="rightHand">Transform for the right hand</param>
        /// <param name="leftHand">Transform for the left hand</param>
        /// <param name="target">The target to hit</param>
        /// <param name="instigator">The GameObject that initiated the projectile</param>
        /// <param name="calulatedDamage">The amount of damage the projectile should deal</param>
        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calulatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calulatedDamage);
        }
        
        /// <summary>
        /// Checks if this weapon has a projectile component.
        /// </summary>
        /// <returns>True if the weapon has a projectile, false otherwise</returns>
        public bool HasProjectile() => projectile != null;

        /// <summary>
        /// Gets the base damage value for this weapon.
        /// </summary>
        /// <returns>The weapon's base damage</returns>
        public float GetDamage() => weaponDamage;

        /// <summary>
        /// Gets the effective range of this weapon.
        /// </summary>
        /// <returns>The weapon's range</returns>
        public float GetRange() => weaponRange;

        /// <summary>
        /// Gets the percentage bonus to damage for this weapon.
        /// </summary>
        /// <returns>The weapon's percentage damage bonus</returns>
        public float GetPercentDamageBonus() => percentDamageBonus;

        /// <summary>
        /// Gets the name of this weapon configuration.
        /// </summary>
        /// <returns>The weapon's name</returns>
        public string GetWeaponName() => name;

        /// <summary>
        /// Gets this weapon configuration instance.
        /// </summary>
        /// <returns>This WeaponConfig instance</returns>
        public WeaponConfig GetWeapon() => this;
    }
}