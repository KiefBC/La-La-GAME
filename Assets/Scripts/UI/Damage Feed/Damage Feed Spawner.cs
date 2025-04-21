using UnityEngine;

namespace UI.Damage_Feed
{
    public class DamageFeedSpawner : MonoBehaviour
    {
        [SerializeField] private DamageFeed damageFeedPrefab = null;

        public void Spawn(float damage)
        {
            DamageFeed damageFeed = Instantiate<DamageFeed>(damageFeedPrefab, transform);
            damageFeed.SetValue(damage);
        }
    }
}
