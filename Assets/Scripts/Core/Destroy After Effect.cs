using UnityEngine;

namespace Core
{
    /// <summary>
    /// Monitors a ParticleSystem and destroys either itself or a specified target GameObject
    /// once the particle effect has completed playing.
    /// </summary>
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] private GameObject targetToDestroy = null;

        /// <summary>
        /// Checks if the ParticleSystem has finished playing each frame.
        /// If the effect is no longer alive, destroys either the specified target
        /// or this GameObject if no target is set.
        /// </summary>
        private void Update()
        {
            if (!GetComponent<ParticleSystem>().IsAlive())
            {
                Destroy(targetToDestroy != null ? targetToDestroy : gameObject);
            }
        }
    }
}