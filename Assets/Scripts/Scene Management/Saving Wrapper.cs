using System;
using System.Collections;
using UnityEngine;
using Core.Saving;

namespace Scene_Management
{
    public class SavingWrapper : MonoBehaviour
    {
        private const string SaveFileName = "sav1";
        [SerializeField] private float fadeInTime = 1f;

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            Fader fader = FindAnyObjectByType<Fader>();
            fader.FadeOutImmediate();
            yield return GetComponent<JsonSavingSystem>().LoadLastScene(SaveFileName);
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
            GetComponent<JsonSavingSystem>().Delete(SaveFileName);
        }

        public void Load()
        {
            GetComponent<JsonSavingSystem>().Load(SaveFileName);
        }
        
        public void Save()
        {
            GetComponent<JsonSavingSystem>().Save(SaveFileName);
        }
    }
}
