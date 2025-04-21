using UnityEngine;
using System.Collections.Generic;

namespace Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "The Game/Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] private ProgressionCharacterClass[] characterClasses = null;
        private Dictionary<CharacterClass, Dictionary<Stat, float[]>> _lookUpTable = null;
        
        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookUpTable();
            
            // Check if character class exists
            if (!_lookUpTable.ContainsKey(characterClass))
            {
                Debug.LogWarning($"Character class {characterClass} not found in progression");
                return 0;
            }
            
            // Check if stat exists for this character class
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
