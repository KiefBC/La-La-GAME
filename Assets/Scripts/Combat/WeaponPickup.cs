using System.Collections;
using Attributes;
using Control;
using UnityEngine;
using UnityEngine.Serialization;

namespace Combat
{
    public class WeaponPickup : MonoBehaviour, IRayCastable
    {
        [SerializeField] private WeaponConfig weaponConfig = null;
        [SerializeField] private float hideTime = 5f;
        [SerializeField] private float pickupRange = 2f;
        [SerializeField] private float healthRestore = 0;
        
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

        private void Spin()
        {
            transform.Rotate(0, 30 * Time.deltaTime, 0);
        }
        
        void Update()
        {
            Spin();
        }

        private IEnumerator HideForSeconds(float time)
        {
            ShowPickUp(false);
            yield return new WaitForSeconds(time);
            ShowPickUp(true);
        }

        private void ShowPickUp(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public CursorState GetCursorState()
        {
            return CursorState.Pickup;
        }

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
