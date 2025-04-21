using TMPro;
using UnityEngine;

namespace Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        private BaseStats _baseStats;

        private void Awake()
        {
            _baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }
        
        private void Update()
        {
            GetComponent<TextMeshProUGUI>().text = $"{_baseStats.GetLevel():0}";
        }
    }
}
