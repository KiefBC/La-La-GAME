using Core.Saving;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Attributes
{
    public class Experience : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private float xp = 0f;

        public void GainExperience(float xpGain)
        {
            xp += xpGain;
        }
        
        public float GetExperiencePoints()
        {
            return xp;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(xp);
        }

        public void RestoreFromJToken(JToken state)
        {
            xp = state.ToObject<float>();
            Debug.Log("Restored XP: " + xp);
        }
    }
}
