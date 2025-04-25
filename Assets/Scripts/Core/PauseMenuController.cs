using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Core.Saving;
using Control;

namespace Core
{
    /// <summary>
    /// Controls the pause menu functionality, including pausing/resuming the game,
    /// saving/loading game state, and handling quit operations.
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private UnityEvent onPause;
        [SerializeField] private UnityEvent onResume;
        [SerializeField] private UnityEvent onSave;
        
        private bool _isPaused = false;
        private static bool _isTransitioning = false;
        private PlayerController _playerController;

        /// <summary>
        /// Initializes the pause menu in a disabled state and gets a reference to the player controller.
        /// </summary>
        private void Awake()
        {
            pauseMenuUI.SetActive(false);
            _playerController = FindAnyObjectByType<PlayerController>();
        }

        /// <summary>
        /// Ensures the game starts in an unpaused state with normal time scale.
        /// </summary>
        private void Start()
        {
            Time.timeScale = 1f;
            _isPaused = false;
        }

        /// <summary>
        /// Checks for the escape key press to toggle the pause state.
        /// </summary>
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

        /// <summary>
        /// Pauses the game by showing the pause menu, setting time scale to 0,
        /// and triggering the pause event.
        /// </summary>
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

        /// <summary>
        /// Resumes the game by hiding the pause menu, restoring time scale to 1,
        /// and triggering the resume event.
        /// </summary>
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

        /// <summary>
        /// Loads the game state from a save file and optionally resumes the game.
        /// </summary>
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

        /// <summary>
        /// Saves the current game state to a save file.
        /// </summary>
        public void SaveGame()
        {
            var savingSystem = FindAnyObjectByType<JsonSavingSystem>();
            if (savingSystem != null)
            {
                savingSystem.Save("sav");
                Debug.Log("Game Saved");
            }
        }

        /// <summary>
        /// Returns to the main menu scene.
        /// </summary>
        // TODO not done yet
        public void QuitToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// Quits the application or stops play mode in the Unity Editor.
        /// </summary>
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