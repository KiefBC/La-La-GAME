using UnityEngine;
using System.Collections.Generic;

namespace Stats
{
    /// <summary>
    /// ScriptableObject that defines how character stats progress with levels.
    /// Manages stat progression for different character classes.
    /// </summary>
    [CreateAssetMenu(fileName = "Progression", menuName = "The Game/Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] private ProgressionCharacterClass[] characterClasses = null;
        private Dictionary<CharacterClass, Dictionary<Stat, float[]>> _lookUpTable = null;
        
        /// <summary>
        /// Retrieves the value of a specific stat for a character class at a given level.
        /// </summary>
        /// <param name="stat">The stat to look up</param>
        /// <param name="characterClass">The character class</param>
        /// <param name="level">The level to look up</param>
        /// <returns>The stat value for the given parameters</returns>
        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookUpTable();
            
            if (!_lookUpTable.ContainsKey(characterClass))
            {
                Debug.LogWarning($"Character class {characterClass} not found in progression");
                return 0;
            }
            
            if (!_lookUpTable[characterClass].ContainsKey(stat))
            {
                Debug.LogWarning($"Stat {stat} not found for character class {characterClass}");
                return 0;
            }
            
            float[] levels = _lookUpTable[characterClass][stat];
            if (levels.Length < level)
            {
                Debug.LogWarning($"Level {level} not found for {stat} in character class {characterClass}");
                return 0;
            }
            return levels[level - 1];
        }

        /// <summary>
        /// Builds the lookup table for quick stat access if not already built.
        /// </summary>
        private void BuildLookUpTable()
        {
            if (_lookUpTable != null) return;
            _lookUpTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
            
            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                var statLookUpTable = new Dictionary<Stat, float[]>();
                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookUpTable[progressionStat.stat] = progressionStat.levels;
                }
                
                _lookUpTable[progressionClass.characterClass] = statLookUpTable;
            }
        }
        
        /// <summary>
        /// Gets the maximum level defined for a specific stat and character class.
        /// </summary>
        /// <param name="stat">The stat to check</param>
        /// <param name="characterClass">The character class</param>
        /// <returns>The maximum level defined for the stat</returns>
        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookUpTable();
            float[] levels = _lookUpTable[characterClass][stat];
            return levels.Length;
        }
        
        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }
        
        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}
