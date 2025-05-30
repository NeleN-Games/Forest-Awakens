using System.IO;
using Base_Classes;
using Editor.CategoryTool;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// A Unity Editor window used to create new collectable item prefabs,
    /// including associated scripts and asset references like icon and world sprites.
    /// </summary>
    public class SourcePrefabCreatorWindow : EditorWindow
    {
        private string itemName = "";
        private Sprite iconSprite;
        private Sprite worldSprite;
        private bool isCheckingSpelling;

        /// <summary>
        /// Opens the Source Prefab Creator window from the Unity Editor menu.
        /// </summary>
        [MenuItem("Tools/Source Prefab Creator")]
        public static void ShowWindow()
        {
            GetWindow<SourcePrefabCreatorWindow>("Source Prefab Creator");
        }

        /// <summary>
        /// Renders the UI layout and handles user interactions in the editor window.
        /// </summary>
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Create New Collectable Item", EditorStyles.boldLabel);

            itemName = EditorGUILayout.TextField("Source Name", itemName);
            iconSprite = (Sprite)EditorGUILayout.ObjectField("Icon Sprite", iconSprite, typeof(Sprite), false);
            worldSprite = (Sprite)EditorGUILayout.ObjectField("World Sprite", worldSprite, typeof(Sprite), false);
            

            GUI.enabled = !isCheckingSpelling;

            if (GUILayout.Button("Create Source Prefab"))
            {
                if (ValidateFields())
                {
                    CreateItem();
                }
            }

            GUI.enabled = true;

            if (isCheckingSpelling)
            {
                EditorGUILayout.HelpBox("Checking spelling...", MessageType.Info);
            }
        }

        /// <summary>
        /// Validates that all necessary fields (name, icon, and sprite) are filled.
        /// </summary>
        /// <returns>True if all fields are valid; otherwise, false.</returns>
        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(itemName) || iconSprite == null || worldSprite == null)
            {
                EditorUtility.DisplayDialog("Error", "All fields must be filled correctly!", "OK");
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Creates a new collectable item prefab and its script after validating inputs
        /// and checking the spelling of the item name.
        /// </summary>
        private void CreateItem()
        {
            OnlineSpellChecker.CheckSpelling(itemName, correctedName =>
            {
                    itemName = correctedName;

                    // Capitalize the first letter of itemName
                    if (!string.IsNullOrEmpty(itemName))
                    {
                        itemName = char.ToUpper(itemName[0]) + itemName.Substring(1);
                    }
                    
                    // Create GameObject
                    GameObject itemGO = new GameObject(itemName);

                    // Add SpriteRenderer
                    SpriteRenderer sr = itemGO.GetComponent<SpriteRenderer>();
                    if (sr == null)
                        sr = itemGO.AddComponent<SpriteRenderer>();
                    sr.sprite = worldSprite;

                    // Add BoxCollider2D
                    if (itemGO.GetComponent<BoxCollider2D>() == null)
                        itemGO.AddComponent<BoxCollider2D>();

                    // Create Collectable Component Script
                    string scriptPath = $"Assets/Scripts/Models/Sources/{itemName}.cs";
                    Directory.CreateDirectory("Assets/Scripts/Models/Sources");

                    if (!File.Exists(scriptPath))
                    {
                        string scriptContent =
$@"using UnityEngine;
using Base_Classes;
namespace Models.Sources
{{
    public class {itemName} : Collectable
    {{
    // Auto-generated collectable item script
    }}
}}";
                        File.WriteAllText(scriptPath, scriptContent);
                        AssetDatabase.ImportAsset(scriptPath);
                    }

                    // Add script component dynamically
                    System.Type newType = typeof(Collectable);
                    var scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
                    if (scriptAsset != null)
                    {
                        newType = scriptAsset.GetClass();
                    }

                    if (newType != null && newType.IsSubclassOf(typeof(Collectable)))
                    {
                        itemGO.AddComponent(newType);
                    }

                    // Save as Prefab
                    string prefabPath = $"Assets/Prefabs/Sources/{itemName}.prefab";
                    Directory.CreateDirectory("Assets/Prefabs/Sources");
                    PrefabUtility.SaveAsPrefabAsset(itemGO, prefabPath);

                    DestroyImmediate(itemGO);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    EditorUtility.DisplayDialog("Success", $"Source prefab '{itemName}' created successfully!", "Nice");
            });
        }
    }
}

