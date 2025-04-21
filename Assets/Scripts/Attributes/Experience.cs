using UnityEngine;

namespace Attributes
{
    public class Experience : MonoBehaviour
    {
        [SerializeField] private float xp = 0f;

        public void GainExperience(float xpGain)
        {
            xp += xpGain;
        }
    }
}
