using Attributes;
using Control;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Marks an entity as a valid combat target and handles combat-related raycast interactions.
    /// Requires a Health component to function.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRayCastable
    {
        /// <summary>
        /// Returns the cursor state when hovering over this combat target.
        /// </summary>
        /// <returns>CursorState.Combat to indicate this is a valid combat target</returns>
        public CursorState GetCursorState()
        {
            return CursorState.Combat;
        }

        /// <summary>
        /// Handles player interaction with this combat target through raycasting.
        /// Initiates combat if the player clicks and can attack this target.
        /// </summary>
        /// <param name="callingController">The PlayerController initiating the interaction</param>
        /// <returns>True if the interaction was handled, false otherwise</returns>
        public bool HandleRaycast(PlayerController callingController)
        {
            if (!callingController.GetComponent<Fighter>().CanAttack(gameObject))
            {
                return false;
            }
                
            if (Input.GetMouseButton(0))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }
            return true;
        }
    }
}