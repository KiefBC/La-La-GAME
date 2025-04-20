using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [Header("Animation")]
        [SerializeField] AnimatorOverrideController playerAnimatorOverride = null;
        [SerializeField] AnimatorOverrideController enemyAnimatorOverride = null;
        
        [Header("Weapon Settings")]
        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] private float weaponDamage = 5f;
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile = null;

        private const string WeaponName = "Weapon";
        
        
        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            if (equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                GameObject weapon = Instantiate(equippedPrefab, handTransform);
                weapon.name = WeaponName;
            }

            if (animator != null)
            {
                AnimatorOverrideController overrideToUse = GetAnimatorOverride(animator.gameObject);
                if (overrideToUse == null) return;
                animator.runtimeAnimatorController = overrideToUse;
            }
        }

        private AnimatorOverrideController GetAnimatorOverride(GameObject owner)
        {
            return owner.CompareTag("Player") ? playerAnimatorOverride : enemyAnimatorOverride;
        }

        private static void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = GetOldWeapon(rightHand, leftHand);
            if (oldWeapon == null) return;
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private static Transform GetOldWeapon(Transform rightHand, Transform leftHand)
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

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, weaponDamage);
        }
        
        public bool HasProjectile() => projectile != null;
        public float GetDamage() => weaponDamage;
        public float GetRange() => weaponRange;
        
        public string GetWeaponName() => name;
        
        public Weapon GetWeapon() => this;
    }
}