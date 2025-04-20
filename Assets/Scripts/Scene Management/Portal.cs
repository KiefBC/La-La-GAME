using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace Scene_Management
{

    enum DestinationIdentifier
    {
        A, B, C, D, E
    }
    public class Portal : MonoBehaviour
    {
        [SerializeField] private int sceneToLoad = -1; // Throw an error if not set
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private DestinationIdentifier destination;
        [SerializeField] private float fadeTime = 1f; // Or we could use fadeIn and fadeOut
        [SerializeField] private float waitTime = 0.5f;

        private Fader _fader;
        
        private void Awake()
        {
            _fader = FindAnyObjectByType<Fader>();
            if (_fader == null)
            {
                Debug.LogError($"Missing Fader component on {gameObject.name}");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set on " + gameObject.name);
                return;
            }
            
            if (!other.gameObject.CompareTag("Player")) return;
            StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set on " + gameObject.name);
                yield break;
            }

            DontDestroyOnLoad(gameObject);
            
            yield return _fader.FadeOut(fadeTime);
            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            
            yield return new WaitForSeconds(waitTime);
            yield return _fader.FadeIn(fadeTime);
            
            Debug.Log("Scene Loaded");
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position); // Teleport the player to the other portal
            player.transform.rotation = otherPortal.spawnPoint.rotation; // Rotate the player to the other portal
            
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsByType<Portal>(FindObjectsSortMode.None))
            {
                if (portal == this) continue; // Not the same portal
                if (portal.destination != destination) continue; // Not the same destination
                    
                return portal;
            }
            return null;
        }
    }
}
