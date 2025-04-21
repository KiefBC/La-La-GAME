using UnityEngine;

namespace Core
{
    public class PersistentObjSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject persistentObjPrefab;
        
        static bool _hasSpawned = false;

        private void Start()
        {
            if (_hasSpawned) return;
            SpawnPersistentObjects();
            _hasSpawned = true;
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentGameObject = Instantiate(persistentObjPrefab);
            DontDestroyOnLoad(persistentGameObject);
        }
    }
}
