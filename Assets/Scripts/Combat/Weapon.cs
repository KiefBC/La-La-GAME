using Core;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] private float weaponDamage = 5f;
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile = null;
        // if is a ranged weapon
        
        
        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            if (equippedPrefab == null) return; // If no prefab, do nothing
            
            Debug.Log("DEBUG :: Spawned weapon");
            Transform handTransform = GetTransform(rightHand, leftHand);
            Instantiate(equippedPrefab, handTransform);
            
            if (animatorOverride == null) return; // If no override, do nothing
            animator.runtimeAnimatorController = animatorOverride;
            Debug.Log("DEBUG :: Overrided animator");
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
    }
}