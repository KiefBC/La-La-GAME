using System.Collections;
using System.Linq;
using UnityEngine;
using Core.Saving;

namespace Scene_Management
{
    /// <summary>
    /// Provides a wrapper around the saving system to handle scene-specific save/load operations
    /// and manage fade transitions during loading.
    /// </summary>
    public class SavingWrapper : MonoBehaviour
    {
        private const string SaveFileName = "sav";
        [SerializeField] private float fadeInTime = 1f;
        private JsonSavingSystem _savingSystem;

        /// <summary>
        /// Initializes the saving system and starts the initial scene load process.
        /// </summary>
        private void Awake()
        {
            _savingSystem = GetComponent<JsonSavingSystem>();
            StartCoroutine(LoadLastScene());
        }

        /// <summary>
        /// Loads the last saved scene with a fade transition and creates an initial save if none exists.
        /// </summary>
        /// <returns>IEnumerator for coroutine execution</returns>
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

        /// <summary>
        /// Checks if a save file exists with the default save name.
        /// </summary>
        /// <returns>True if a save file exists, false otherwise</returns>
        private bool HasExistingSave()
        {
            return _savingSystem.ListSaves().Any(save => save == SaveFileName);
        }

        /// <summary>
        /// Handles input for save, load, and delete operations during gameplay.
        /// </summary>
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

        /// <summary>
        /// Deletes the current save file.
        /// </summary>
        private void Delete()
        {
            _savingSystem.Delete(SaveFileName);
        }

        /// <summary>
        /// Loads the game state from the save file.
        /// </summary>
        public void Load()
        {
            _savingSystem.Load(SaveFileName);
        }
        
        /// <summary>
        /// Saves the current game state to the save file.
        /// </summary>
        public void Save()
        {
            _savingSystem.Save(SaveFileName);
        }
    }
}