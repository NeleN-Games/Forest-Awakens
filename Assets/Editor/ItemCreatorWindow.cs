using System.Collections.Generic;
using System.Linq;
using Enums;
using Models;
using Models.Data;
using Models.Scriptable_Objects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ItemCreatorWindow : EditorWindow
    {
        private string itemName = "";
        private string itemDescription = "";
        private Sprite displaySprite;
        private List<SourceRequirement> resourceRequirements = new();
        private CraftItemDatabase itemDatabase;

        [MenuItem("Tools/Item Creator")]
        public static void ShowWindow()
        {
            GetWindow<ItemCreatorWindow>("Item Creator");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Create Craftable Item", EditorStyles.boldLabel);

            itemName = EditorGUILayout.TextField("Item Name", itemName);
            itemDescription = EditorGUILayout.TextField("Description", itemDescription);
            displaySprite = (Sprite)EditorGUILayout.ObjectField("Display Sprite", displaySprite, typeof(Sprite), false);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Required Resources", EditorStyles.boldLabel);

            if (GUILayout.Button("Add Resource"))
            {
                resourceRequirements.Add(new SourceRequirement());
            }

            for (int i = 0; i < resourceRequirements.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                resourceRequirements[i].sourceType = (SourceType)EditorGUILayout.EnumPopup(resourceRequirements[i].sourceType);
                resourceRequirements[i].amount = EditorGUILayout.IntField(resourceRequirements[i].amount);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    resourceRequirements.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            itemDatabase = (CraftItemDatabase)EditorGUILayout.ObjectField("Item Database", itemDatabase, typeof(CraftItemDatabase), false);

            if (GUILayout.Button("Create Item"))
            {
                if (itemDatabase == null)
                {
                    EditorUtility.DisplayDialog("Missing Database", "Please assign an item database.", "OK");
                    return;
                }

                CraftItemData newItem = CreateInstance<CraftItemData>();
                newItem.itemName = itemName;
                newItem.description = itemDescription;
                newItem.displaySprite = displaySprite;
                newItem.requiredResources = new List<SourceRequirement>(resourceRequirements);

                string path = $"Assets/Scriptable Objects/Items/{itemName}.asset";
                AssetDatabase.CreateAsset(newItem, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (!itemDatabase.items.Any(i => i.itemName == itemName))
                {
                    itemDatabase.items.Add(newItem);
                    EditorUtility.SetDirty(itemDatabase);
                    AssetDatabase.SaveAssets();
                }

                EditorUtility.DisplayDialog("Success", $"Item '{itemName}' created and added to database.", "Nice!");
            }
        }
    }
}
