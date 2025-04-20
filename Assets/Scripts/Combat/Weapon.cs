using Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

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
        
        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            if (equippedPrefab == null) return;
            Transform handTransform = GetTransform(rightHand, leftHand);

            Instantiate(equippedPrefab, handTransform);
            if (animatorOverride == null) return;
            animator.runtimeAnimatorController = animatorOverride;
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            handTransform = isRightHanded ? rightHand : leftHand;
            return handTransform;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target);
        }
        
        public bool HasProjectile() => projectile != null;
        public float GetDamge() => weaponDamage;
        public float GetRange() => weaponRange;
    }
}