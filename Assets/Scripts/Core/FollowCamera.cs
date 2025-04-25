using UnityEngine;

namespace Core
{
    /// <summary>
    /// Controls camera movement by following a specified target transform.
    /// Updates the camera position every frame after all other updates have completed.
    /// </summary>
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
    
        /// <summary>
        /// Updates the camera position to match the target's position.
        /// Called after all Update functions have been called, ensuring smooth camera following.
        /// </summary>
        void LateUpdate()
        {
            transform.position = target.position;
        }
    }
}