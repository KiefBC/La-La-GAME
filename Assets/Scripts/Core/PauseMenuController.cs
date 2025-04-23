using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Core.Saving;
using Control;

namespace Core
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private UnityEvent onPause;
        [SerializeField] private UnityEvent onResume;
        [SerializeField] private UnityEvent onSave;
        
        private bool _isPaused = false;
        private static bool _isTransitioning = false;
        private PlayerController _playerController;

        private void Awake()
        {
            pauseMenuUI.SetActive(false);
            _playerController = FindAnyObjectByType<PlayerController>();
        }

        private void Start()
        {
            Time.timeScale = 1f;
            _isPaused = false;
        }

        private void Update()
        {
            if (_isTransitioning) return;
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Pause()
        {
            if (_isTransitioning || _isPaused) return;
            
            _isTransitioning = true;
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            _isPaused = true;
            _playerController.SetCursorState(CursorState.None);
            onPause?.Invoke();
            _isTransitioning = false;
        }

        public void Resume()
        {
            if (_isTransitioning || !_isPaused) return;
            
            _isTransitioning = true;
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            _isPaused = false;
            onResume?.Invoke();
            _isTransitioning = false;
        }

        public void LoadGame()
        {
            var savingSystem = FindAnyObjectByType<JsonSavingSystem>();
            if (savingSystem != null)
            {
                savingSystem.Load("sav");
                Debug.Log("Game Loaded");
                Resume(); // Optional: automatically resume after loading
            }
        }

        public void SaveGame()
        {
            var savingSystem = FindAnyObjectByType<JsonSavingSystem>();
            if (savingSystem != null)
            {
                savingSystem.Save("sav");
                Debug.Log("Game Saved");
            }
        }

        // TODO not done yet
        public void QuitToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}