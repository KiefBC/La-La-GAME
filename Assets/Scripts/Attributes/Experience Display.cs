using System;
using Stats;
using UnityEngine;
using TMPro;

namespace Attributes
{
    public class ExperienceDisplay : MonoBehaviour
    {
        private Experience _experience;

        private void Awake()
        {
            _experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }
        
        private void Update()
        {
            GetComponent<TextMeshProUGUI>().text = $"{_experience.GetExperiencePoints():0}";
        }
    }
}
