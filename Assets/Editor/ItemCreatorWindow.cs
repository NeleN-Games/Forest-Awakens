using System;
using System.Collections.Generic;
using System.IO;
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
        private string _itemName = "";
        private string _itemDescription = "";
        private Sprite _displaySprite;
        private readonly List<SourceRequirement> _resourceRequirements = new();
        private ItemDatabase _itemDatabase;
        private bool _enumReady = false;
        private int _selectedItemIndex = 0; 
        private  const string EnumPath = "Assets/Scripts/Enums/ItemType.cs";

        [MenuItem("Tools/Item Creator")]
        public static void ShowWindow()
        {
            GetWindow<ItemCreatorWindow>("Item Creator");
        }
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");
            DrawSectionHeader("Item information", new Color(0.2f, 0.5f, 0.8f, 1f));

            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical("box");
            DrawItemFields();
            DrawDatabaseField();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical("box");
            
            DrawSectionHeader("Item Configuration", new Color(0.1f, 0.5f, 0.1f, 1f));
            EditorGUILayout.Space(10);

            DrawResourceRequirements();
            EditorGUILayout.Space(10);

            DrawEnumButton();
            
            EditorGUILayout.Space(10);
            
            DrawCreateButton();
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical("box");
            DrawSectionHeader("Delete Existing Item", new Color(0.8f, 0.1f, 0.1f, 1f));
            DrawDeleteSection();
            EditorGUILayout.EndVertical();
            
        }
        
        #region GUI Drawing Sections
        
        private void DrawItemFields()
        {
            EditorGUI.BeginChangeCheck();
            _itemName = CapitalizeFirstLetter(EditorGUILayout.TextField("Item Name", _itemName));
            if (EditorGUI.EndChangeCheck()) _enumReady = false;

            _itemDescription = EditorGUILayout.TextField("Description", _itemDescription);
            _displaySprite = (Sprite)EditorGUILayout.ObjectField("Display Sprite", _displaySprite, typeof(Sprite), false);
            EditorGUILayout.Space();
        }
        private void DrawResourceRequirements()
        {   
            var allSourceTypes = Enum.GetValues(typeof(SourceType)).Cast<SourceType>();
            var selectedTypes = _resourceRequirements.Select(r => r.sourceType).ToHashSet();
            bool canAdd = selectedTypes.Count < allSourceTypes.Count();

            EditorGUI.BeginDisabledGroup(!canAdd);
            if (GUILayout.Button(new GUIContent("Add Resource", EditorGUIUtility.IconContent("d_Toolbar Plus").image), GUILayout.Height(25)))
            {
                var remaining = allSourceTypes.Except(selectedTypes).ToList();
                if (remaining.Count > 0)
                    _resourceRequirements.Add(new SourceRequirement { sourceType = remaining[0], amount = 1 });
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginVertical("box");
            for (int i = 0; i < _resourceRequirements.Count; i++)
            {
                DrawRequirementRow(i);
            }
            EditorGUILayout.EndVertical();
        }
        private void DrawRequirementRow(int i)
        {
            EditorGUILayout.BeginHorizontal();
            var otherTypes = _resourceRequirements
                .Where((_, index) => index != i)
                .Select(r => r.sourceType)
                .ToHashSet();

            var available = Enum.GetValues(typeof(SourceType)).Cast<SourceType>()
                .Where(t => !otherTypes.Contains(t) || t == _resourceRequirements[i].sourceType).ToList();

            int index = available.IndexOf(_resourceRequirements[i].sourceType);
            int newIndex = EditorGUILayout.Popup(index, available.Select(t => t.ToString()).ToArray());
            if (newIndex >= 0)
            {
                _resourceRequirements[i].sourceType = available[newIndex];
            }

            _resourceRequirements[i].amount = EditorGUILayout.IntField(_resourceRequirements[i].amount);

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _resourceRequirements.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        private void DrawDatabaseField()
        {
            EditorGUILayout.Space();
            _itemDatabase = (ItemDatabase)EditorGUILayout.ObjectField("Item Database", _itemDatabase, typeof(ItemDatabase), false);
        }
        private void DrawEnumButton()
        {
            if (GUILayout.Button("Check or Add Enum"))
            {
                if (string.IsNullOrWhiteSpace(_itemName))
                {
                    EditorUtility.DisplayDialog("Error", "Item name cannot be empty.", "OK");
                    return;
                }

                if (Enum.TryParse<ItemType>(_itemName, out _))
                {
                    EditorUtility.DisplayDialog("Info", $"'{_itemName}' already exists in ItemType enum.", "OK");
                    _enumReady = false;
                }
                else
                {
                    SyncEnumWithDatabase();
                    AddItemTypeToEnum(_itemName);
                    _enumReady = true;
                    EditorUtility.DisplayDialog("Added", $"'{_itemName}' was added to ItemType enum.\nYou can now create the item.", "OK");
                }
            }
        }
        private void DrawCreateButton()
        {
            var canCreate=_enumReady && _resourceRequirements.Count > 0;
            GUI.enabled = canCreate;
            var createBtnStyle = new GUIStyle(GUI.skin.button);
            if (canCreate) createBtnStyle.normal.textColor = Color.green;
            if (GUILayout.Button("Create Item", createBtnStyle))
            {
                if (_itemDatabase == null)
                {
                    EditorUtility.DisplayDialog("Missing Database", "Please assign an item database.", "OK");
                    return;
                }
                CreateItem();
                SyncEnumWithDatabase();
                _enumReady = false;
            }
            GUI.enabled = true;
            
        }
        private void DrawDeleteSection()
        {
            bool canDelete = _itemDatabase != null && _itemDatabase.Entries.Count > 0;
            GUI.enabled = canDelete;

            if (canDelete)
            {
                var itemNames = _itemDatabase.Entries.Select(i => i.name).ToArray();
                _selectedItemIndex = EditorGUILayout.Popup("Select Item", _selectedItemIndex, itemNames);

                if (GUILayout.Button("Delete Selected Item") && IsValidIndex(_selectedItemIndex, itemNames.Length))
                {
                    var item = _itemDatabase.Entries[_selectedItemIndex];
                    if (EditorUtility.DisplayDialog("Confirm Delete", $"Delete '{item.name}' and assets?", "Yes", "No"))
                    {
                        DeleteItemAndAssets(item);
                        SyncEnumWithDatabase();
                        _selectedItemIndex = 0;
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No items found in the database.");
            }
            GUI.enabled = true;
        }
        private bool IsValidIndex(int index, int length) => index >= 0 && index < length;
        
        #endregion
      
        #region Helper Methods
      
        private string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            if (input.Length == 1)
                return input.ToUpper();
            return char.ToUpper(input[0]) + input.Substring(1);
        }

        
        private void EnsureFolder(string path)
        {
            var split = path.Split('/');
            string current = split[0];
            for (int i = 1; i < split.Length; i++)
            {
                string next = current + "/" + split[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, split[i]);
                current = next;
            }
        }
        private void CreateItem()
        {
            if (_itemDatabase == null)
            {
                EditorUtility.DisplayDialog("Missing Database", "Please assign an item database.", "OK");
                return;
            }
            
            EnsureFolder("Assets/Prefabs/Items");
            EnsureFolder("Assets/Scriptable Objects/Items");

            GameObject itemObject = new GameObject(_itemName);
            itemObject.AddComponent<SpriteRenderer>().sprite = _displaySprite;


            string prefabPath = $"Assets/Prefabs/Items/{_itemName}.prefab";
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(itemObject, prefabPath);
            DestroyImmediate(itemObject);

            ItemData newItem = CreateInstance<ItemData>();
            newItem.name = _itemName;
            if (!Enum.TryParse(_itemName, out ItemType itemType))
            {
                Debug.LogError($"Enum.Parse failed: '{_itemName}' not found in ItemType after refresh.");
                return;
            }
            newItem.type = itemType;
            newItem.description = _itemDescription;
            newItem.icon = _displaySprite;
            newItem.prefab = prefab;
            newItem.requirements = new List<SourceRequirement>(_resourceRequirements);

            string assetPath = $"Assets/Scriptable Objects/Items/{_itemName}.asset";
            AssetDatabase.CreateAsset(newItem, assetPath);

            _itemDatabase.Entries.Add(newItem);
            EditorUtility.SetDirty(_itemDatabase);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Success", $"Item '{_itemName}' created and added to database." +
                                                   $"\nItemData path is {assetPath}." +
                                                   $"\nPrefab path is {prefabPath}", "Nice!");
        }
        
        private void AddItemTypeToEnum(string itemName)
        {
            if (!File.Exists(EnumPath))
            {
                Debug.LogError("Enum file not found."); return;
            }

            string[] lines = File.ReadAllLines(EnumPath);
            if (lines.Any(l => l.Contains(itemName)))
            {
                Debug.LogWarning($"Enum already contains {itemName}");
                return;
            }

            int insertIndex = Array.FindIndex(lines, l => l.Contains("enum ItemType")) + 2;
            if (insertIndex < 2)
            {
                Debug.LogError("ItemType enum not found."); return;
            }

            var newLines = lines.ToList();
            newLines.Insert(insertIndex, $"        {itemName},");
            File.WriteAllLines(EnumPath, newLines);
            AssetDatabase.Refresh();
            Debug.Log($"Added '{itemName}' to ItemType enum.");
        }
        private void SyncEnumWithDatabase()
        {
            if (_itemDatabase == null)
            {
                Debug.LogWarning("ItemDatabase is null. Cannot sync enum.");
                return;
            }

            List<string> itemNamesInDb = GetAllItemNamesFromDatabase();

            if (!File.Exists(EnumPath))
            {
                Debug.LogError("Enum file not found.");
                return;
            }

            var lines = File.ReadAllLines(EnumPath).ToList();

            int enumStartIndex = lines.FindIndex(l => l.Contains("enum ItemType"));
            if (enumStartIndex == -1)
            {
                Debug.LogError("ItemType enum not found.");
                return;
            }

            int startInsertIndex = enumStartIndex + 2;

            var enumLines = new List<string>();
            int i = startInsertIndex;
            while (i < lines.Count && !lines[i].Trim().StartsWith("}"))
            {
                enumLines.Add(lines[i]);
                i++;
            }

            var currentEnumMembers = enumLines
                .Select(line => line.Trim().TrimEnd(',').Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            var newEnumMembers = new HashSet<string>(itemNamesInDb);

            bool modified = false;

            for (int j = enumLines.Count - 1; j >= 0; j--)
            {
                var memberName = enumLines[j].Trim().TrimEnd(',').Trim();
                if (!newEnumMembers.Contains(memberName))
                {
                    lines.RemoveAt(startInsertIndex + j);
                    modified = true;
                }
            }

            currentEnumMembers = lines.Skip(startInsertIndex).TakeWhile(l => !l.Trim().StartsWith("}"))
                .Select(l => l.Trim().TrimEnd(',').Trim()).ToList();

            foreach (var itemName in newEnumMembers)
            {
                if (!currentEnumMembers.Contains(itemName))
                {
                    lines.Insert(startInsertIndex, $"        {itemName},");
                    startInsertIndex++;
                    modified = true;
                }
            }

            if (modified)
            {
                File.WriteAllLines(EnumPath, lines);
                AssetDatabase.Refresh();
                Debug.Log("ItemType enum synced with database.");
            }
            else
            {
                Debug.Log("ItemType enum already synced.");
            }
        }
        
        private List<string> GetAllItemNamesFromDatabase()
        {
            if (_itemDatabase == null)
            {
                Debug.LogWarning("ItemDatabase is null.");
                return new List<string>();
            }

            if (_itemDatabase.Entries == null || _itemDatabase.Entries.Count == 0)
            {
                Debug.LogWarning("ItemDatabase.Entries is empty.");
                return new List<string>();
            }
            List<string> itemNames = new List<string>();

            foreach (var item in _itemDatabase.Entries)
            {
                if (!string.IsNullOrEmpty(item.name))
                    itemNames.Add(CapitalizeFirstLetter(item.name)); 
            }

            return itemNames;
        }

        private void RemoveItemTypeFromEnum(string itemName)
        {
            string enumPath = EnumPath;
            if (!File.Exists(enumPath))
            {
                Debug.LogError("Enum file not found.");
                return;
            }

            var lines = File.ReadAllLines(enumPath).ToList();
            int enumStartIndex = lines.FindIndex(l => l.Contains("enum ItemType"));
            if (enumStartIndex == -1)
            {
                Debug.LogError("ItemType enum not found.");
                return;
            }

            int startIndex = enumStartIndex + 2;
            int endIndex = lines.FindIndex(startIndex, l => l.Trim().StartsWith("}"));
            if (endIndex == -1) endIndex = lines.Count;

            bool removed = false;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (lines[i].Trim().TrimEnd(',').Trim() == itemName)
                {
                    lines.RemoveAt(i);
                    removed = true;
                    break;
                }
            }

            if (removed)
            {
                File.WriteAllLines(enumPath, lines);
                AssetDatabase.Refresh();
                Debug.Log($"Removed '{itemName}' from ItemType enum.");
            }
            else
            {
                Debug.LogWarning($"'{itemName}' not found in enum.");
            }
        }

        private void DeleteItemAndAssets(ItemData item)
        {
            if (_itemDatabase.Remove(item))  
            {
                EditorUtility.SetDirty(_itemDatabase);
                RemoveItemTypeFromEnum(item.name);
                
                DeleteAssetIfExists(item.prefab);
                DeleteAssetIfExists(item);
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Deleted", $"Selected Item and its assets have been deleted.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", $"Failed to delete item from database.", "OK");
            }
        }
        private void DeleteAssetIfExists(UnityEngine.Object obj)
        {
            if (obj == null) return;
            string path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path))
                AssetDatabase.DeleteAsset(path);
        }
        private void DrawSectionHeader(string title, Color bgColor)
        {
            var rect = EditorGUILayout.GetControlRect(false, 20);
            EditorGUI.DrawRect(rect, bgColor);
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
            };
            GUI.Label(rect, title, style);
        }

        #endregion
    }
}
