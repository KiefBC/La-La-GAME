using UnityEngine;

namespace UI.Damage_Feed
{
    /// <summary>
    /// Handles the spawning of damage number displays in the UI.
    /// Creates instances of damage feed prefabs and initializes them with damage values.
    /// </summary>
    public class DamageFeedSpawner : MonoBehaviour
    {
        [SerializeField] private DamageFeed damageFeedPrefab = null;

        /// <summary>
        /// Spawns a new damage number display as a child of this object.
        /// </summary>
        /// <param name="damage">The amount of damage to display in the spawned text</param>
        public void Spawn(float damage)
        {
            DamageFeed damageFeed = Instantiate<DamageFeed>(damageFeedPrefab, transform);
            damageFeed.SetValue(damage);
        }
    }
}