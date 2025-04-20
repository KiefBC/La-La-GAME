using UnityEngine;

namespace Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon weapon = null;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            if (other.GetComponent<Fighter>().GetWeapon() != weapon)
            {
                other.GetComponent<Fighter>().EquipWeapon(weapon);
                Destroy(gameObject);
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
    }
}
