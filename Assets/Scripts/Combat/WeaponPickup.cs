using System.Collections;
using UnityEngine;

namespace Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon weapon = null;
        [SerializeField] private float hideTime = 5f;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            if (other.GetComponent<Fighter>().GetWeapon() != weapon)
            {
                other.GetComponent<Fighter>().EquipWeapon(weapon);
                // Destroy(gameObject);
                StartCoroutine(HideForSeconds(hideTime));
            }
            else
            {
                Debug.Log("DEBUG :: Already have this weapon");
            }
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

        private void ShowPickUp(bool shouldshow)
        {
            GetComponent<Collider>().enabled = true;
            
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldshow);
            }
        }
    }
}
