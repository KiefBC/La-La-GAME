using System.Collections;
using UnityEngine;

namespace Scene_Management
{
    /// <summary>
    /// Manages screen fade transitions using a CanvasGroup component.
    /// Provides methods for immediate and gradual fading in/out of screen overlays.
    /// </summary>
    public class Fader : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private Coroutine _currentActiveFade;
        
        /// <summary>
        /// Initializes the fader by getting the required CanvasGroup component.
        /// Logs an error if the CanvasGroup component is missing.
        /// </summary>
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                Debug.LogError($"Missing CanvasGroup component on {gameObject.name}");
            }
        }
        
        /// <summary>
        /// Immediately sets the screen to fully faded out (black).
        /// Useful for initial scene states or instant transitions.
        /// </summary>
        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1;
        }

        /// <summary>
        /// Initiates a fade out transition over the specified duration.
        /// </summary>
        /// <param name="time">Duration of the fade in seconds</param>
        /// <returns>IEnumerator for coroutine execution</returns>
        public IEnumerator FadeOut(float time)
        {
            return Fade(1, time);
        }

        /// <summary>
        /// Performs a fade transition to a target alpha value over the specified duration.
        /// Cancels any currently running fade operation before starting.
        /// </summary>
        /// <param name="target">Target alpha value (0 = transparent, 1 = opaque)</param>
        /// <param name="time">Duration of the fade in seconds</param>
        /// <returns>IEnumerator for coroutine execution</returns>
        public IEnumerator Fade(float target, float time)
        {
            if (_currentActiveFade != null)
            {
                StopCoroutine(_currentActiveFade);
            }
            _currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            yield return _currentActiveFade;
        }

        /// <summary>
        /// Internal routine that handles the actual fade transition animation.
        /// Gradually adjusts the canvas group alpha towards the target value.
        /// </summary>
        /// <param name="target">Target alpha value (0 = transparent, 1 = opaque)</param>
        /// <param name="time">Duration of the fade in seconds</param>
        /// <returns>IEnumerator for coroutine execution</returns>
        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(_canvasGroup.alpha, target))
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }

        /// <summary>
        /// Initiates a fade in transition over the specified duration.
        /// </summary>
        /// <param name="time">Duration of the fade in seconds</param>
        /// <returns>IEnumerator for coroutine execution</returns>
        public IEnumerator FadeIn(float time)
        {
            return Fade(0, time);
        }
    }
}