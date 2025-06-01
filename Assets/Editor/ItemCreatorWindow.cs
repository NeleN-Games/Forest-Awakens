using System.Collections.Generic;
using System.Linq;
using Databases;
using Enums;
using Models;
using Models.Data;
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
        private ItemDatabase itemDatabase;

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
            itemDatabase = (ItemDatabase)EditorGUILayout.ObjectField("Item Database", itemDatabase, typeof(ItemDatabase), false);

            if (GUILayout.Button("Create Item"))
            {
                if (itemDatabase == null)
                {
                    EditorUtility.DisplayDialog("Missing Database", "Please assign an item database.", "OK");
                    return;
                }

                ItemData newItem = CreateInstance<ItemData>();
                
                //todo : create new itemtype
                //newItem.type = itemName;
                
                newItem.description = itemDescription;
                newItem.icon = displaySprite;
                newItem.requirements = new List<SourceRequirement>(resourceRequirements);

                string path = $"Assets/Scriptable Objects/Items/{itemName}.asset";
                AssetDatabase.CreateAsset(newItem, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (itemDatabase.Entries.All(i => i.type.ToString() != itemName))
                {
                    itemDatabase.Entries.Add(newItem);
                    EditorUtility.SetDirty(itemDatabase);
                    AssetDatabase.SaveAssets();
                }

                EditorUtility.DisplayDialog("Success", $"Item '{itemName}' created and added to database.", "Nice!");
            }
        }
    }
}
