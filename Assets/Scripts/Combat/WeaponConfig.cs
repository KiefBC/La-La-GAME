using Attributes;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Combat
{
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

        private AnimatorOverrideController GetAnimatorOverride(GameObject owner)
        {
            return owner.CompareTag("Player") ? playerAnimatorOverride : enemyAnimatorOverride;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = GetOldWeapon(rightHand, leftHand);
            if (oldWeapon == null) return;
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(WeaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(WeaponName);
            }
            return oldWeapon;
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform = isRightHanded ? rightHand : leftHand;
            return handTransform;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calulatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calulatedDamage);
        }
        
        public bool HasProjectile() => projectile != null;
        public float GetDamage() => weaponDamage;
        public float GetRange() => weaponRange;
        public float GetPercentDamageBonus() => percentDamageBonus;
        public string GetWeaponName() => name;
        public WeaponConfig GetWeapon() => this;
    }
}