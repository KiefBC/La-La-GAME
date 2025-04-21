using System.Collections;
using UnityEngine;
using Core.Saving;

namespace Scene_Management
{
    public class SavingWrapper : MonoBehaviour
    {
        const string saveFileName = "sav1";
        [SerializeField] private float fadeInTime = 1f;
        
        IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return GetComponent<JsonSavingSystem>().LoadLastScene(saveFileName);
            yield return fader.FadeIn(fadeInTime);
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        private void Delete()
        {
            GetComponent<JsonSavingSystem>().Delete(saveFileName);
        }

        public void Load()
        {
            GetComponent<JsonSavingSystem>().Load(saveFileName);
        }
        
        public void Save()
        {
            GetComponent<JsonSavingSystem>().Save(saveFileName);
        }
    }
}
