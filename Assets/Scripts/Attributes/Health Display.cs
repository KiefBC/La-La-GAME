using System;
using Stats;
using UnityEngine;
using TMPro;

namespace Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        private Health _health;

        private void Awake()
        {
            _health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }
        
        private void Update()
        {
            GetComponent<TextMeshProUGUI>().text = $"{_health.GetHealthPercentage():0}%";
        }
    }
}
