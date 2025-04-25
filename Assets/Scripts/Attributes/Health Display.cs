using UnityEngine;
using TMPro;

namespace Attributes
{
    /// <summary>
    /// Displays the player's current and maximum health in a TextMeshPro UI element.
    /// Updates the display continuously to reflect changes in health values.
    /// </summary>
    public class HealthDisplay : MonoBehaviour
    {
        private Health _health;

        /// <summary>
        /// Initializes the component by finding and storing a reference to the player's Health component.
        /// </summary>
        private void Awake()
        {
            _health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }
        
        /// <summary>
        /// Updates the TextMeshPro text component with the current and maximum health values.
        /// Format: "CurrentHealth/MaxHealth"
        /// </summary>
        private void Update()
        {
            GetComponent<TextMeshProUGUI>().text = $"{_health.GetHealthPoints():0}/{_health.GetMaxHealthPoints():0}";
        }
    }
}