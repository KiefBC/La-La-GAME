using Attributes;
using TMPro;
using UnityEngine;

namespace Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter _fighter;

        private void Awake()
        {
            _fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }
        
        private void Update()
        {
            if (_fighter.GetTarget() == null)
            {
                GetComponent<TextMeshProUGUI>().text = "N/A";
                return;
            }
            Health _health = _fighter.GetTarget().GetComponent<Health>();
            GetComponent<TextMeshProUGUI>().text = $"{_health.GetHealthPoints():0}/{_health.GetMaxHealthPoints():0}"; 
        }
    }
}
