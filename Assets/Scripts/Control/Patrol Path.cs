using UnityEngine;

namespace Control
{
    public class PatrolPath : MonoBehaviour
    {
        private const float WayPointRadius = 0.25f;

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
        
        public int GetNextIndex(int index)
        {
            if (index + 1 == transform.childCount) return 0;
            return index + 1;
        }

        public Vector3 GetWayPoint(int index)
        {
            return transform.GetChild(index).position;
        }
    }
}
