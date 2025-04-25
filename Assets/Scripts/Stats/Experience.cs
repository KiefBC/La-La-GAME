using System;
using Core.Saving;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Stats
{
    /// <summary>
    /// Manages character experience points and level progression.
    /// Handles experience gain and persistence of experience data.
    /// </summary>
    public class Experience : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private float experiencePoints = 0f;
        
        public event Action OnExperienceGained;

        /// <summary>
        /// Adds experience points and triggers the experience gained event.
        /// </summary>
        /// <param name="xpGain">Amount of experience to add</param>
        public void GainExperience(float xpGain)
        {
            experiencePoints += xpGain;
            OnExperienceGained();
        }
        
        /// <summary>
        /// Returns the current amount of experience points.
        /// </summary>
        /// <returns>Current experience points</returns>
        public float GetExperiencePoints()
        {
            return experiencePoints;
        }

        /// <summary>
        /// Serializes the experience points for saving.
        /// </summary>
        /// <returns>JToken containing experience data</returns>
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(experiencePoints);
        }

        /// <summary>
        /// Restores experience points from saved data.
        /// </summary>
        /// <param name="state">JToken containing saved experience data</param>
        public void RestoreFromJToken(JToken state)
        {
            experiencePoints = state.ToObject<float>();
            Debug.Log("Restored XP: " + experiencePoints);
        }
    }
}