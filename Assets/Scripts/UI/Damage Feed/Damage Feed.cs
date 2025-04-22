using UnityEngine;
using TMPro;

namespace UI.Damage_Feed
{
    public class DamageFeed : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI damageText = null;

        public void DestroyText()
        {
            Destroy(gameObject);
        }
        
        public void SetValue(float amount)
        {
            damageText.text = $"{amount:0}";
        }
    }
}
