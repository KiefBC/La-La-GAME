using Attributes;
using TMPro;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Displays the health of the currently targeted enemy in a TextMeshPro UI element.
    /// </summary>
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter _fighter;

        /// <summary>
        /// Initializes the component by finding and storing a reference to the player's Fighter component.
        /// </summary>
        private void Awake()
        {
            _fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }
        
        /// <summary>
        /// Updates the health display text every frame.
        /// Displays "N/A" when no target is selected, otherwise shows current/max health of the target.
        /// </summary>
        private void Update()
        {
            if (_fighter.GetTarget() == null)
            {
                GetComponent<TextMeshProUGUI>().text = "N/A";
                return;
            }
            Health health = _fighter.GetTarget().GetComponent<Health>();
            GetComponent<TextMeshProUGUI>().text = $"{health.GetHealthPoints():0}/{health.GetMaxHealthPoints():0}"; 
        }
    }
}