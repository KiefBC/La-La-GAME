using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private UnityEvent hitEvent;

        public void OnHit()
        {
            Debug.Log("DEBUG :: Hit");
            hitEvent.Invoke();
        }
    }
}
