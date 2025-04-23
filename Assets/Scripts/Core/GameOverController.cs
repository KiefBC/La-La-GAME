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
    public class GameOverController : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverMenuUI;
        [SerializeField] private UnityEvent onGameOver;
        
        private static bool _isTransitioning = false;
        private PlayerController _playerController;
        private SavingWrapper _savingWrapper;

        private void Awake()
        {
            gameOverMenuUI.SetActive(false);
            _playerController = FindAnyObjectByType<PlayerController>();
            _savingWrapper = FindAnyObjectByType<SavingWrapper>();
        }

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

        public void LoadLastSave()
        {
            RestartScene();
        }

        private IEnumerator LoadLastSaveRoutine(Fader fader)
        {
            yield return fader.FadeOut(1f);
            _savingWrapper.Load();
            yield return fader.FadeIn(1f);
            
            LoadingScreen.Instance.Hide();
            Resume();
        }

        private void Resume()
        {
            if (_isTransitioning) return;
            
            _isTransitioning = true;
            gameOverMenuUI.SetActive(false);
            Time.timeScale = 1f;
            _isTransitioning = false;
        }

        private bool HasSaveFile(JsonSavingSystem savingSystem)
        {
            return savingSystem.ListSaves().Any(save => save == "sav");
        }

        private void RestartScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }

        public void ReturnToMainMenu()
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