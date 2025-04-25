using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    /// <summary>
    /// Represents a weapon in the game, handling weapon-specific events and behaviors.
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private UnityEvent hitEvent;

        /// <summary>
        /// Called when the weapon successfully hits a target.
        /// Triggers the hitEvent and logs a debug message.
        /// </summary>
        public void OnHit()
        {
            Debug.Log("DEBUG :: Hit");
            hitEvent.Invoke();
        }
    }
}