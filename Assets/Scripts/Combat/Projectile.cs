using Core;
using UnityEngine;

namespace Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;
    
        private Health _target = null;
    
    
        void Update()
        {
            if (!_target) return;
            transform.LookAt(GetAimPosition());
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        }
        
        public void SetTarget(Health target)
        {
            if (!target) return;
            Debug.Log("DEBUG :: Projectile target set");
            this._target = target;
        }

        private Vector3 GetAimPosition()
        {
            CapsuleCollider targetCollider = _target.GetComponent<CapsuleCollider>();
            if (!targetCollider) return _target.transform.position; // If no collider, just aim at the target
            return _target.transform.position + targetCollider.center;
        }
    }
}
