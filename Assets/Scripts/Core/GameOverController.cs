using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Core.Saving;
using System.Linq;
using Control;
using Scene_Management;

namespace Core
{
    /// <summary>
    /// Manages the game over state and UI, handling scene transitions, save loading,
    /// and game state management when the player dies or the game ends.
    /// </summary>
    public class GameOverController : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverMenuUI;
        [SerializeField] private UnityEvent onGameOver;
        
        private static bool _isTransitioning = false;
        private PlayerController _playerController;
        private SavingWrapper _savingWrapper;

        /// <summary>
        /// Initializes required components and ensures the game over UI is hidden on startup.
        /// </summary>
        private void Awake()
        {
            gameOverMenuUI.SetActive(false);
            _playerController = FindAnyObjectByType<PlayerController>();
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        }

        /// <summary>
        /// Displays the game over UI, pauses the game, and triggers the game over event.
        /// Prevents multiple simultaneous transitions.
        /// </summary>
        public void ShowGameOver()
        {
            if (_isTransitioning) return;
            
            _isTransitioning = true;
            gameOverMenuUI.SetActive(true);
            Time.timeScale = 0f;
            _playerController.SetCursorState(CursorState.UI);
            onGameOver?.Invoke();
            _isTransitioning = false;
        }

        /// <summary>
        /// Loads the last saved game state by restarting the current scene.
        /// </summary>
        public void LoadLastSave()
        {
            RestartScene();
        }

        /// <summary>
        /// Coroutine that handles the fade transition when loading the last save.
        /// </summary>
        /// <param name="fader">The Fader component used for screen transitions</param>
        /// <returns>IEnumerator for coroutine execution</returns>
        private IEnumerator LoadLastSaveRoutine(Fader fader)
        {
            yield return fader.FadeOut(1f);
            _savingWrapper.Load();
            yield return fader.FadeIn(1f);
            
            LoadingScreen.Instance.Hide();
            Resume();
        }

        /// <summary>
        /// Resumes the game by hiding the game over UI and restoring normal time scale.
        /// Prevents multiple simultaneous transitions.
        /// </summary>
        private void Resume()
        {
            if (_isTransitioning) return;
            
            _isTransitioning = true;
            gameOverMenuUI.SetActive(false);
            Time.timeScale = 1f;
            _isTransitioning = false;
        }

        /// <summary>
        /// Checks if a save file exists in the specified save system.
        /// </summary>
        /// <param name="savingSystem">The JsonSavingSystem to check for saves</param>
        /// <returns>True if a save file exists, false otherwise</returns>
        private bool HasSaveFile(JsonSavingSystem savingSystem)
        {
            return savingSystem.ListSaves().Any(save => save == "sav");
        }

        /// <summary>
        /// Restarts the current scene by reloading it.
        /// </summary>
        private void RestartScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }

        /// <summary>
        /// Returns to the main menu (scene index 0) and restores normal time scale.
        /// </summary>
        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// Quits the game application, handling both editor and built application cases.
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