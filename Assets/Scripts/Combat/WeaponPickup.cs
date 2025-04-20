using UnityEngine;

namespace Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon weapon = null;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
        
            Debug.Log("DEBUG :: Picked up weapon");
            other.GetComponent<Fighter>().EquipWeapon(weapon);
            Destroy(gameObject);
        }
    }
}
