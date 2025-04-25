using UnityEngine;
using TMPro;

namespace UI.Damage_Feed
{
    /// <summary>
    /// Manages the display of damage numbers in the UI.
    /// Handles text value setting and cleanup of damage number instances.
    /// </summary>
    public class DamageFeed : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI damageText = null;

        /// <summary>
        /// Destroys the GameObject this component is attached to,
        /// removing the damage number from the scene.
        /// </summary>
        public void DestroyText()
        {
            Destroy(gameObject);
        }
        
        /// <summary>
        /// Sets the displayed damage value in the TextMeshPro component.
        /// </summary>
        /// <param name="amount">The damage amount to display, formatted as a whole number</param>
        public void SetValue(float amount)
        {
            damageText.text = $"{amount:0}";
        }
    }
}