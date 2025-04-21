using System;
using UnityEngine;


namespace Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 20)]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression = null;
        [SerializeField] private GameObject levelUpParticleEffect = null;
        [SerializeField] private bool shouldUseModifiers = false;
        

        public event Action OnLevelUp;

        private int _currentLevel = 0;

        private void Start()
        {
            _currentLevel = CalculateLevel();
            Experience experience = GetComponent<Experience>();
            if (experience != null)
            {
                experience.OnExperienceGained += UpdateLevel;
            }
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

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

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > _currentLevel)
            {
                _currentLevel = newLevel;
                Debug.Log("Level up! You are now level " + _currentLevel);
                LevelUpParticleEffect();
                OnLevelUp();
            }
        }

        private void LevelUpParticleEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        public int GetLevel()
        {
            if (_currentLevel < 1)
            {
                _currentLevel = CalculateLevel();
            }
            return _currentLevel;
        }

        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;
            
            float currentXP =experience.GetExperiencePoints();
            int maxLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= maxLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (currentXP < XPToLevelUp)
                {
                    return level;
                }
            }
            return maxLevel + 1;
        }

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
