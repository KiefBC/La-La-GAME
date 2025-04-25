using UnityEngine;

namespace Core
{
    /// <summary>
    /// Makes an object always face the main camera by matching its forward direction.
    /// Useful for billboarding effects or ensuring UI elements/sprites always face the camera.
    /// </summary>
    public class CameraFacing : MonoBehaviour
    {
        /// <summary>
        /// Updates the object's forward direction each frame to match the main camera's forward direction.
        /// This ensures the object is always facing the camera regardless of camera movement.
        /// </summary>
        void Update()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}