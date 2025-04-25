using TMPro;
using UnityEngine;

namespace Stats
{
    /// <summary>
    /// Handles the UI display of the player's experience points.
    /// Updates the display continuously to show current experience.
    /// </summary>
    public class ExperienceDisplay : MonoBehaviour
    {
        private Experience _experience;

        /// <summary>
        /// Initializes the component by finding the player's Experience component.
        /// </summary>
        private void Awake()
        {
            _experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }
        
        /// <summary>
        /// Updates the experience display text each frame.
        /// </summary>
        private void Update()
        {
            GetComponent<TextMeshProUGUI>().text = $"{_experience.GetExperiencePoints():0}";
        }
    }
}