using System.Collections;
using System.Linq;
using UnityEngine;
using Core.Saving;

namespace Scene_Management
{
    public class SavingWrapper : MonoBehaviour
    {
        private const string SaveFileName = "sav";
        [SerializeField] private float fadeInTime = 1f;
        private JsonSavingSystem _savingSystem;

        private void Awake()
        {
            _savingSystem = GetComponent<JsonSavingSystem>();
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            yield return _savingSystem.LoadLastScene(SaveFileName);
            Fader fader = FindAnyObjectByType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
            
            // Create initial save point only if no save exists
            if (!HasExistingSave())
            {
                Save();
            }
        }

        private bool HasExistingSave()
        {
            return _savingSystem.ListSaves().Any(save => save == SaveFileName);
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
            _savingSystem.Delete(SaveFileName);
        }

        public void Load()
        {
            _savingSystem.Load(SaveFileName);
        }
        
        public void Save()
        {
            _savingSystem.Save(SaveFileName);
        }
    }
}