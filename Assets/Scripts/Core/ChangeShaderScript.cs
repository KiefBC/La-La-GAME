using UnityEditor;
using UnityEngine;

namespace Core
{
    public class ChangeShaderScript : EditorWindow
    {
        private Shader _targetShader;

        [MenuItem("Tools/Batch Change Shader")]
        static void Init()
        {
            ChangeShaderScript window = (ChangeShaderScript)EditorWindow.GetWindow(typeof(ChangeShaderScript));
            window.titleContent = new GUIContent("Change Shader");
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("Select a shader to apply to all selected objects", EditorStyles.boldLabel);
        
            _targetShader = (Shader)EditorGUILayout.ObjectField("Target Shader", _targetShader, typeof(Shader), false);
        
            if (_targetShader == null)
            {
                // Try to find Standard shader by default
                _targetShader = Shader.Find("Standard");
            }
        
            EditorGUILayout.Space();
        
            if (GUILayout.Button("Apply to Selected Objects"))
            {
                ChangeShaderForSelected();
            }
        
            if (GUILayout.Button("Apply to All Scene Objects"))
            {
                ChangeShaderForAllObjects();
            }
        }
    
        void ChangeShaderForSelected()
        {
            if (_targetShader == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a shader first!", "OK");
                return;
            }
        
            GameObject[] selectedObjects = Selection.gameObjects;
            int count = 0;
        
            foreach (GameObject obj in selectedObjects)
            {
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material mat in renderer.sharedMaterials)
                    {
                        if (mat != null)
                        {
                            mat.shader = _targetShader;
                            count++;
                        }
                    }
                }
            }
        
            Debug.Log($"Changed shader on {count} materials to {_targetShader.name}");
        }
    
        void ChangeShaderForAllObjects()
        {
            if (_targetShader == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a shader first!", "OK");
                return;
            }
        
            Renderer[] allRenderers = GameObject.FindObjectsOfType<Renderer>();
            int count = 0;
        
            foreach (Renderer renderer in allRenderers)
            {
                foreach (Material mat in renderer.sharedMaterials)
                {
                    if (mat != null)
                    {
                        mat.shader = _targetShader;
                        count++;
                    }
                }
            }
        
            Debug.Log($"Changed shader on {count} materials to {_targetShader.name}");
        }
    }
}