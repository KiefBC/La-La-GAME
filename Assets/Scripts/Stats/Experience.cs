using System;
using Core.Saving;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Stats
{
    public class Experience : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private float experiencePoints = 0f;
        
        public event Action OnExperienceGained;

        public void GainExperience(float xpGain)
        {
            experiencePoints += xpGain;
            OnExperienceGained();
        }
        
        public float GetExperiencePoints()
        {
            return experiencePoints;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(experiencePoints);
        }

        public void RestoreFromJToken(JToken state)
        {
            experiencePoints = state.ToObject<float>();
            Debug.Log("Restored XP: " + experiencePoints);
        }

    }
}
