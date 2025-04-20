using UnityEngine;

namespace Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon weapon = null;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            // If we already have the same weapon
            if (other.GetComponent<Fighter>().GetWeapon() != weapon)
            {
                Debug.Log("DEBUG :: Picked up weapon");
                other.GetComponent<Fighter>().EquipWeapon(weapon);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("DEBUG :: Already have this weapon");
            }
        }
    }
}
