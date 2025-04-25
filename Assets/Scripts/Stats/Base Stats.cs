using System;
using UnityEngine;

namespace Stats
{
    /// <summary>
    /// Manages the base statistics for a character, including level progression
    /// and stat calculations with modifiers.
    /// </summary>
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression = null;
        [SerializeField] private GameObject levelUpParticleEffect = null;
        [SerializeField] private bool shouldUseModifiers = false;

        private Experience _experience;
        
        public event Action OnLevelUp;
        private int _currentLevel = 0;

        /// <summary>
        /// Initializes required components.
        /// </summary>
        private void Awake()
        {
            _experience = GetComponent<Experience>();
        }

        /// <summary>
        /// Sets initial level on start.
        /// </summary>
        private void Start()
        {
            _currentLevel = CalculateLevel();
        }

        /// <summary>
        /// Subscribes to experience gain events.
        /// </summary>
        private void OnEnable()
        {
            if (_experience != null)
            {
                _experience.OnExperienceGained += UpdateLevel;
            }
        }
        
        /// <summary>
        /// Unsubscribes from experience gain events.
        /// </summary>
        private void OnDisable()
        {
            if (_experience != null)
            {
                _experience.OnExperienceGained -= UpdateLevel;
            }
        }

        /// <summary>
        /// Calculates the final value of a stat including all modifiers.
        /// </summary>
        /// <param name="stat">The stat to calculate</param>
        /// <returns>The final calculated stat value</returns>
        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        /// <summary>
        /// Gets the current level of the character.
        /// </summary>
        /// <returns>Current character level</returns>
        public int GetLevel()
        {
            if (_currentLevel < 1)
            {
                _currentLevel = CalculateLevel();
            }
            return _currentLevel;
        }

        /// <summary>
        /// Calculates the percentage modifier for a stat.
        /// </summary>
        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        /// <summary>
        /// Gets the base value of a stat before modifiers.
        /// </summary>
        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        /// <summary>
        /// Updates the level when experience changes and triggers level up events.
        /// </summary>
        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > _currentLevel)
            {
                _currentLevel = newLevel;
                LevelUpParticleEffect();
                OnLevelUp?.Invoke();
            }
        }

        /// <summary>
        /// Spawns the level up particle effect.
        /// </summary>
        private void LevelUpParticleEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        /// <summary>
        /// Calculates the current level based on experience points.
        /// </summary>
        /// <returns>The calculated level</returns>
        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;
            
            float currentXp = experience.GetExperiencePoints();
            int maxLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= maxLevel; level++)
            {
                float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (currentXp < xpToLevelUp)
                {
                    return level;
                }
            }
            return maxLevel + 1;
        }

        /// <summary>
        /// Calculates the additive modifier for a stat.
        /// </summary>
        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }
    }
}