using UnityEngine;

namespace Core
{
    /// <summary>
    /// Manages the spawning of persistent objects that should exist throughout the game's lifetime.
    /// Ensures only one instance of the persistent object is created across scene loads.
    /// </summary>
    public class PersistentObjSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject persistentObjPrefab;
        
        static bool _hasSpawned = false;

        /// <summary>
        /// Initializes the persistent object if it hasn't been spawned yet.
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Start()
        {
            if (_hasSpawned) return;
            SpawnPersistentObjects();
            _hasSpawned = true;
        }

        /// <summary>
        /// Creates an instance of the persistent object prefab and marks it to persist across scene loads.
        /// </summary>
        private void SpawnPersistentObjects()
        {
            GameObject persistentGameObject = Instantiate(persistentObjPrefab);
            DontDestroyOnLoad(persistentGameObject);
        }
    }
}