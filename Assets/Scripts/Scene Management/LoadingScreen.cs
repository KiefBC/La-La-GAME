using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Scene_Management
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image progressBar;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private float fadeTime = 0.5f;
        
        private static LoadingScreen instance;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            Hide();
        }

        public static LoadingScreen Instance => instance;

        public void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(FadeRoutine(1f));
        }

        public void Hide()
        {
            StartCoroutine(FadeRoutine(0f));
        }

        public void SetProgress(float progress)
        {
            if (progressBar != null) progressBar.fillAmount = progress;
            if (progressText != null) progressText.text = $"{Mathf.Round(progress * 100)}%";
        }

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