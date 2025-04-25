using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace Scene_Management
{
    /// <summary>
    /// Handles scene transitions through portal objects, managing player teleportation
    /// and scene loading with fade effects.
    /// </summary>
    public class Portal : MonoBehaviour
    {
        [SerializeField] private int sceneToLoad = -1; // Scene build index to load
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private DestinationIdentifier destination;
        [SerializeField] private float fadeTime = 1f;
        [SerializeField] private float waitTime = 1f;

        /// <summary>
        /// Identifies matching portals between scenes for correct player spawning.
        /// </summary>
        private enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        /// <summary>
        /// Triggers the scene transition when the player enters the portal's trigger zone.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        /// <summary>
        /// Handles the complete scene transition process including saving, loading,
        /// and player teleportation.
        /// </summary>
        private IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set on " + gameObject.name);
                yield break;
            }

            // Prepare portal for scene transition
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            
            Fader fader = FindAnyObjectByType<Fader>();
            
            // Fade out current scene
            yield return fader.FadeOut(fadeTime);
            
            // Save current state
            SavingWrapper savingWrapper = FindAnyObjectByType<SavingWrapper>();
            savingWrapper.Save();
            
            // Load new scene
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            
            // Restore saved state
            savingWrapper.Load();

            // Update player position
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            
            // Save new state
            savingWrapper.Save();
            
            // Wait and fade in
            yield return new WaitForSeconds(waitTime);
            yield return fader.FadeIn(fadeTime);
            
            Destroy(gameObject);
        }

        /// <summary>
        /// Finds the matching portal in the newly loaded scene.
        /// </summary>
        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsByType<Portal>(FindObjectsSortMode.None))
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;
                return portal;
            }
            return null;
        }

        /// <summary>
        /// Updates the player's position to the destination portal's spawn point.
        /// </summary>
        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null || otherPortal == null) return;

            NavMeshAgent navMeshAgent = player.GetComponent<NavMeshAgent>();
            if (navMeshAgent != null)
            {
                navMeshAgent.Warp(otherPortal.spawnPoint.position);
                player.transform.rotation = otherPortal.spawnPoint.rotation;
            }
        }
    }
}