using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Scene_Management
{
    /// <summary>
    /// Manages the loading screen UI, including progress bar, text updates, and fade transitions.
    /// Implements the Singleton pattern to ensure only one instance exists.
    /// </summary>
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image progressBar;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private float fadeTime = 0.5f;
        
        private static LoadingScreen _instance;
        
        /// <summary>
        /// Initializes the singleton instance and sets up the loading screen.
        /// Ensures only one instance exists and hides the screen on startup.
        /// </summary>
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            Hide();
        }

        /// <summary>
        /// Gets the singleton instance of the LoadingScreen.
        /// </summary>
        public static LoadingScreen Instance => _instance;

        /// <summary>
        /// Shows the loading screen with a fade-in animation.
        /// Activates the game object and starts the fade coroutine.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(FadeRoutine(1f));
        }

        /// <summary>
        /// Hides the loading screen with a fade-out animation.
        /// </summary>
        public void Hide()
        {
            StartCoroutine(FadeRoutine(0f));
        }

        /// <summary>
        /// Updates the progress bar and text display with the current loading progress.
        /// </summary>
        /// <param name="progress">Loading progress value between 0 and 1</param>
        public void SetProgress(float progress)
        {
            if (progressBar != null) progressBar.fillAmount = progress;
            if (progressText != null) progressText.text = $"{Mathf.Round(progress * 100)}%";
        }

        /// <summary>
        /// Handles the fade animation for showing/hiding the loading screen.
        /// Uses unscaled time to ensure consistent animation regardless of time scale.
        /// </summary>
        /// <param name="targetAlpha">Target alpha value (0 for fade out, 1 for fade in)</param>
        /// <returns>IEnumerator for coroutine execution</returns>
        private IEnumerator FadeRoutine(float targetAlpha)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeTime);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            if (targetAlpha == 0f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}