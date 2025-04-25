using UnityEngine;

namespace Control
{
    /// <summary>
    /// Manages a patrol path consisting of waypoints for AI navigation.
    /// Waypoints are stored as child objects of this component and can be visualized in the editor using Gizmos.
    /// </summary>
    public class PatrolPath : MonoBehaviour
    {
        /// <summary>
        /// The radius of the sphere Gizmo used to visualize waypoints in the editor.
        /// </summary>
        private const float WayPointRadius = 0.25f;

        /// <summary>
        /// Draws Gizmos in the Unity Editor to visualize the patrol path.
        /// Displays red spheres for waypoints and lines connecting consecutive points.
        /// </summary>
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(GetWayPoint(i), WayPointRadius);
                Gizmos.DrawLine(GetWayPoint(i), GetWayPoint(j));
            }
        }
        
        /// <summary>
        /// Gets the index of the next waypoint in the patrol sequence.
        /// Loops back to the first waypoint if at the end of the sequence.
        /// </summary>
        /// <param name="index">Current waypoint index</param>
        /// <returns>Index of the next waypoint in the sequence</returns>
        public int GetNextIndex(int index)
        {
            if (index + 1 == transform.childCount) return 0;
            return index + 1;
        }

        /// <summary>
        /// Gets the world position of a waypoint at the specified index.
        /// </summary>
        /// <param name="index">Index of the desired waypoint</param>
        /// <returns>World position of the waypoint</returns>
        public Vector3 GetWayPoint(int index)
        {
            return transform.GetChild(index).position;
        }
    }
}