using Core;
using UnityEngine;

namespace Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;
    
        private Health target = null;
    
    
        void Update()
        {
            if (!target) return;
            transform.LookAt(GetAimPosition());
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        }
        
        public void SetTarget(Health target)
        {
            this.target = target;
        }

        private Vector3 GetAimPosition()
        {
            CapsuleCollider targetCollider = GetComponent<CapsuleCollider>();
            if (!targetCollider) return target.transform.position;
            if (targetCollider.center == Vector3.zero) return target.transform.position;
            return target.transform.position + targetCollider.center;
        }
    }
}
