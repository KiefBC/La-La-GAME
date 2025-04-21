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
            Health health = _fighter.GetTarget().GetComponent<Health>();
            GetComponent<TextMeshProUGUI>().text = $"{health.GetHealthPoints():0}/{health.GetMaxHealthPoints():0}"; 
        }
    }
}
