using UnityEngine;

namespace Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 20)]
        [SerializeField] private int level = 1;
        [SerializeField] private CharacterClass characterClass;
    }
}
