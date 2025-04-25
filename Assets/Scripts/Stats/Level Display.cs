using TMPro;
using UnityEngine;

namespace Stats
{
    /// <summary>
    /// Handles the UI display of the player's current level.
    /// Updates the display continuously to show current level.
    /// </summary>
    public class LevelDisplay : MonoBehaviour
    {
        private BaseStats _baseStats;

        /// <summary>
        /// Initializes the component by finding the player's BaseStats component.
        /// </summary>
        private void Awake()
        {
            _baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }
        
        /// <summary>
        /// Updates the level display text each frame.
        /// </summary>
        private void Update()
        {
            GetComponent<TextMeshProUGUI>().text = $"{_baseStats.GetLevel():0}";
        }
    }
}