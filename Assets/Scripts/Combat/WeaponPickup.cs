using System.Collections;
using Attributes;
using Control;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Handles weapon and health pickup functionality, including collision detection,
    /// weapon equipping, and pickup visibility management.
    /// </summary>
    public class WeaponPickup : MonoBehaviour, IRayCastable
    {
        [SerializeField] private WeaponConfig weaponConfig = null;
        [SerializeField] private float hideTime = 5f;
        [SerializeField] private float pickupRange = 2f;
        [SerializeField] private float healthRestore = 0;
        
        /// <summary>
        /// Handles collision-based pickup functionality when the player enters the trigger zone.
        /// Checks for weapon equality and handles both weapon and health pickups.
        /// </summary>
        /// <param name="other">The collider that entered the trigger zone</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            // Only check weapon equality if this pickup has a weapon configuration
            if (weaponConfig != null)
            {
                if (other.GetComponent<Fighter>().GetWeapon() != weaponConfig)
                {
                    Pickup(other.gameObject);
                }
                else
                {
                    Debug.Log("DEBUG :: Already have this weapon");
                }
            }
            else if (healthRestore > 0)
            {
                // If it's just a healing pickup, directly heal without weapon logic
                other.GetComponent<Health>().Heal(healthRestore);
                StartCoroutine(HideForSeconds(hideTime));
            }
        }

        /// <summary>
        /// Processes the pickup action by equipping the weapon and/or restoring health,
        /// then temporarily hides the pickup.
        /// </summary>
        /// <param name="itemToPickup">The GameObject (player) that is picking up the item</param>
        private void Pickup(GameObject itemToPickup)
        {
            if (weaponConfig != null)
            {
                itemToPickup.GetComponent<Fighter>().EquipWeapon(weaponConfig);
            }
            if (healthRestore > 0)
            {
                itemToPickup.GetComponent<Health>().Heal(healthRestore);
            }
            StartCoroutine(HideForSeconds(hideTime));
        }

        /// <summary>
        /// Rotates the pickup object around its Y-axis for visual effect.
        /// </summary>
        private void Spin()
        {
            transform.Rotate(0, 30 * Time.deltaTime, 0);
        }
        
        /// <summary>
        /// Updates the pickup's rotation each frame.
        /// </summary>
        void Update()
        {
            Spin();
        }

        /// <summary>
        /// Temporarily hides the pickup object and its collider for the specified duration.
        /// </summary>
        /// <param name="time">Duration in seconds to hide the pickup</param>
        /// <returns>IEnumerator for coroutine functionality</returns>
        private IEnumerator HideForSeconds(float time)
        {
            ShowPickUp(false);
            yield return new WaitForSeconds(time);
            ShowPickUp(true);
        }

        /// <summary>
        /// Controls the visibility of the pickup object and its collider.
        /// </summary>
        /// <param name="shouldShow">Whether the pickup should be visible and interactive</param>
        private void ShowPickUp(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        /// <summary>
        /// Implements IRayCastable interface to define cursor state when hovering over pickup.
        /// </summary>
        /// <returns>The appropriate cursor state for this pickup</returns>
        public CursorState GetCursorState()
        {
            return CursorState.Pickup;
        }

        /// <summary>
        /// Handles raycast-based pickup interaction when the player clicks on the pickup.
        /// </summary>
        /// <param name="callingController">The PlayerController initiating the raycast</param>
        /// <returns>True if the raycast was handled, false otherwise</returns>
        public bool HandleRaycast(PlayerController callingController)
        {
            float distanceToWeapon = Vector3.Distance(callingController.transform.position, transform.position);

            if (!(distanceToWeapon < pickupRange)) return false;
            if (!Input.GetMouseButton(0)) return true;
            Fighter fighter = callingController.GetComponent<Fighter>();
            if (fighter.GetWeapon() != weaponConfig)
            {
                Pickup(callingController.gameObject);
            }
            else
            {
                Debug.Log("DEBUG :: Already have this weapon");
            }
            return true;
        }
    }
}