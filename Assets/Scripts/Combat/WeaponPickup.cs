using System.Collections;
using Control;
using UnityEngine;
using UnityEngine.Serialization;

namespace Combat
{
    public class WeaponPickup : MonoBehaviour, IRayCastable
    {
        [FormerlySerializedAs("weapon")] [SerializeField] private WeaponConfig weaponConfig = null;
        [SerializeField] private float hideTime = 5f;
        [SerializeField] private float pickupRange = 2f;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            if (other.GetComponent<Fighter>().GetWeapon() != weaponConfig)
            {
                Pickup(other.GetComponent<Fighter>());
            }
            else
            {
                Debug.Log("DEBUG :: Already have this weapon");
            }
        }

        private void Pickup(Fighter fighter)
        {
            fighter.EquipWeapon(weaponConfig);
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

            if (!(distanceToWeapon <= pickupRange)) return false;
            if (!Input.GetMouseButton(0)) return true;
            Fighter fighter = callingController.GetComponent<Fighter>();
            if (fighter.GetWeapon() != weaponConfig)
            {
                Pickup(fighter);
            }
            else
            {
                Debug.Log("DEBUG :: Already have this weapon");
            }
            return true;

        }
    }
}
