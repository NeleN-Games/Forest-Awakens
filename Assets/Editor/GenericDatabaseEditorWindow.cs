using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Databases;
using Editor.Utilities;
using Enums;
using Interfaces;
using Managers;
using Models;
using Models.Data;
using UnityEditor;
using UnityEngine;
using Utilities;
using Object = UnityEngine.Object;

namespace Editor
{
    public abstract class GenericDatabaseEditorWindow<TData, TDatabase, TEnum> : EditorWindow
        where TData :  CommonAssetData<TEnum>, IIdentifiable<TEnum>
        where TDatabase : GenericDatabase<TEnum,TData>
        where TEnum : struct, Enum
    {
        private Sprite _displaySprite;
        private TDatabase _database;
        private string EnumReadyKey => $"EnumReady_{typeof(TEnum).Name}";
        private bool EnumReady
        {
            get => EditorPrefs.GetBool(EnumReadyKey, false);
            set => EditorPrefs.SetBool(EnumReadyKey, value);
        }
        
        private int SelectedItemIndex { get; set; } = 0;
        private int  SelectedCategoryKey  => $"AssetName_SelectedCategory".GetHashCode();
        private CategoryType SelectedCategory
        {
            get
            {
                int storedValue = EditorPrefs.GetInt(SelectedCategoryKey.ToString());
                return (CategoryType)storedValue;
            }
            set => EditorPrefs.SetInt(SelectedCategoryKey.ToString(), (int)value);
        }

        private string AssetNameKey => $"AssetName_{typeof(TEnum).Name}";
        private string AssetName
        {
            get => EditorPrefs.GetString(AssetNameKey, "");
            set => EditorPrefs.SetString(AssetNameKey, value);
        }
        
        private List<SourceRequirement> _resourceRequirements = new();
        private bool RequiresResourceRequirements => SetRequiresResourceRequirements();
        protected abstract bool SetRequiresResourceRequirements();

        private string EditorName=> GetEditorName();
        protected abstract  string GetEditorName();
        /// <summary>
        /// Path to read/write database.
        /// </summary>
        private string DatabaseFolderPath => GetExpectedDatabasePath();   
        protected abstract  string GetExpectedDatabasePath();
        private string Database => GetExpectedDatabaseName();
        protected abstract string GetExpectedDatabaseName();
        
        /// <summary>
        /// Path to save created prefab.
        /// </summary>
        private string PrefabsFolderPath => GetExpectedPrefabsPath();
        protected abstract string GetExpectedPrefabsPath();
        
        /// <summary>
        /// Path to add/remove ScriptableObject like ItemData, SourceData, BuildingData.
        /// </summary>
        private string DataFolderPath => GetExpectedDataPath();
        protected abstract string GetExpectedDataPath();
        
        
        protected string EnumName => typeof(TEnum).Name;
        private string EnumPath =>GetEnumPath();
        protected abstract string GetEnumPath();
        public void Draw()
        {
            EditorGUILayout.BeginVertical("box");
            
            Drawer.DrawSectionHeader($"{EditorName} information", new Color(0.2f, 0.5f, 0.8f, 1f));
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical("box");
            DrawItemFields();
            DrawDatabaseField();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical("box");
            
            Drawer.DrawSectionHeader($"{EditorName} Configuration", new Color(0.1f, 0.5f, 0.1f, 1f));
            
            EditorGUILayout.Space(10);
            
            if (RequiresResourceRequirements)
            {
                _resourceRequirements=SourceRequirementPrefs.Load();
                ResourceRequirementDrawer.Draw(ref _resourceRequirements);

                SelectedCategory = (CategoryType)EditorGUILayout.EnumPopup("Category", SelectedCategory);

                EditorGUILayout.Space();
            }
            DrawEnumButton();
            
            EditorGUILayout.Space(10);
            
            DrawCreateButton();
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical("box");
            Drawer.DrawSectionHeader($"Delete Existing {EditorName}", new Color(0.8f, 0.1f, 0.1f, 1f));
                
            DrawDeleteSection();
            EditorGUILayout.EndVertical();
            
        }
        
        protected virtual void DrawItemFields()
        {
            EditorGUI.BeginChangeCheck();
            AssetName = StringUtility.CapitalizeFirstLetter(EditorGUILayout.TextField(GetNameFieldLabel(), AssetName));
            if (EditorGUI.EndChangeCheck()) EnumReady = false;
            _displaySprite = (Sprite)EditorGUILayout.ObjectField("Display Sprite", _displaySprite, typeof(Sprite), false);
            EditorGUILayout.Space();
        }
        protected abstract string GetNameFieldLabel();

        protected virtual void DrawDatabaseField()
        {
           
            EditorGUILayout.Space();
            _database = (TDatabase)EditorGUILayout.ObjectField($"{EditorName} Database", _database, typeof(GenericDatabase<TEnum,TData>), false);
        }

        protected virtual void DrawEnumButton()
        {
            if (GUILayout.Button("Check or Add Enum"))
            {
                if (string.IsNullOrWhiteSpace(AssetName))
                {
                    EditorUtility.DisplayDialog("Error", $"{EditorName} name cannot be empty.", "OK");
                    return;
                }

                if (Enum.TryParse<TEnum>(AssetName, out _))
                {
                    EditorUtility.DisplayDialog("Info", $"'{AssetName}' already exists in {EnumName} enum.", "OK");
                    EnumReady = false;
                }
                else
                {
                    SyncEnum();
                    ModifyEnumUtility.AddItemTypeToEnum(AssetName,EnumPath,EnumName);
                    EnumReady = true;
                    EditorUtility.DisplayDialog("Added", $"'{AssetName}' was added to {EnumName} enum.\nYou can now create the {EditorName}.", "OK");
                }
            }
        }
        private void SyncEnum()
        {
            if (_database == null)
            {
                Debug.LogWarning("Database is null, cannot sync enum.");
                return;
            }
            EnumSyncUtility.SyncEnumFromDatabase<TEnum,TData,TDatabase>(EnumName, EnumPath, _database);

            AssetDatabase.Refresh();
        }
        
        

        protected virtual void DrawCreateButton()
        {
            var canCreate=EnumReady;
            if (RequiresResourceRequirements)
            {
                canCreate = EnumReady && _resourceRequirements.Count > 0;
            }
            GUI.enabled = canCreate;
            var createBtnStyle = new GUIStyle(GUI.skin.button);
            if (canCreate) createBtnStyle.normal.textColor = Color.green;
            if (GUILayout.Button($"Create {EditorName}", createBtnStyle))
            {
                if (_database == null)
                {
                    EditorUtility.DisplayDialog("Missing Database", $"Please assign an {EditorName} database.", "OK");
                    return;
                }
                CreateItem();
                SyncEnum();
                EnumReady = false;
                SourceRequirementPrefs.RemoveAll();
            }
            GUI.enabled = true;
        }
        private void CreateItem()
        {
            if (_database == null)
            {
                EditorUtility.DisplayDialog("Missing Database", $"Please assign an {EditorName} database.", "OK");
                return;
            }
            
            EnsureFolder(PrefabsFolderPath);
            EnsureFolder(DataFolderPath);
           
            Debug.Log(AssetName);
            GameObject itemObject = new GameObject(AssetName);
            itemObject.AddComponent<SpriteRenderer>().sprite = _displaySprite;
            string prefabPath = $"{PrefabsFolderPath}/{AssetName}.prefab";
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(itemObject, prefabPath); 
            DestroyImmediate(itemObject);
        
            TData newItem = CreateInstance<TData>();
            
            newItem.name = AssetName;
            if (!Enum.TryParse(AssetName, out TEnum itemType))
            {
                Debug.LogError($"Enum.Parse failed: '{AssetName}' not found in {EnumName} after refresh.");
                return;
            }

            var uniqueId = UniqueIdManager.CreateNewUniqueId();
            if (newItem is CraftableAssetData<TEnum> craftableData && RequiresResourceRequirements)
            {
               craftableData.resourceRequirements=new List<SourceRequirement>(_resourceRequirements);
               craftableData.Initialize(prefab,_displaySprite,itemType,_resourceRequirements,SelectedCategory,uniqueId);
            }
            else
            {
                newItem.Initialize(prefab,_displaySprite,itemType);
            }
            string assetPath = $"{DataFolderPath}/{AssetName}.asset";
            AssetDatabase.CreateAsset(newItem, assetPath);

            _database.Entries.Add(newItem);
            EditorUtility.SetDirty(_database);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Success", $"Item '{AssetName}' created and added to database." +
                                                   $"\nItemData path is {assetPath}." +
                                                   $"\nPrefab path is {PrefabsFolderPath}", "Nice!");
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

      protected virtual void DrawDeleteSection()
        {
            bool canDelete = _database != null && _database.Entries.Count > 0;
            GUI.enabled = canDelete;

            if (canDelete)
            {
                var itemNames = _database.Entries.Select(i => i.name).ToArray();
                SelectedItemIndex = EditorGUILayout.Popup("Select Item", SelectedItemIndex, itemNames);

                if (GUILayout.Button("Delete Selected Item") && IsValidIndex(SelectedItemIndex, itemNames.Length))
                {
                    var item = _database.Entries[SelectedItemIndex];
                    if (EditorUtility.DisplayDialog("Confirm Delete", $"Delete '{item.name}' and assets?", "Yes", "No"))
                    {
                        DeleteObjectAndAssets(item);
                        SyncEnum();
                        SelectedItemIndex = 0;
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No items found in the database.");
            }
            GUI.enabled = true;
         }
        private static bool IsValidIndex(int index, int length)
        {
            return index >= 0 && index < length;
        }
        private void DeleteObjectAndAssets(TData source)
        {
            if (_database.Remove(source))  
            {
                EditorUtility.SetDirty(_database);
                RemoveItemTypeFromEnum(source.name);
                
                DeleteAssetIfExists(source.prefab);
                DeleteAssetIfExists(source);
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Deleted", $"Selected {source.name} and its assets have been deleted.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", $"Failed to delete {source.name} from database.", "OK");
            }
        }
        private void DeleteAssetIfExists(UnityEngine.Object obj)
        {
            if (obj == null) return;
            string path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path))
                AssetDatabase.DeleteAsset(path);
        }
        
        private void RemoveItemTypeFromEnum(string objectName)
        {
            string enumPath = EnumPath;
            if (!File.Exists(enumPath))
            {
                Debug.LogError("Enum file not found.");
                return;
            }

            var lines = File.ReadAllLines(enumPath).ToList();
            int enumStartIndex = lines.FindIndex(l => l.Contains($"enum {EnumName}"));
            if (enumStartIndex == -1)
            {
                Debug.LogError($"{EnumName} enum not found.");
                return;
            }

            int startIndex = enumStartIndex + 2;
            int endIndex = lines.FindIndex(startIndex, l => l.Trim().StartsWith("}"));
            if (endIndex == -1) endIndex = lines.Count;

            bool removed = false;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (lines[i].Trim().TrimEnd(',').Trim() == objectName)
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
                Debug.Log($"Removed '{objectName}' from {EnumName} enum.");
            }
            else
            {
                Debug.LogWarning($"'{objectName}' not found in enum.");
            }
        }
        
        protected virtual void OnEnable()
        {
            LoadAssets();
        }

        private void LoadAssets()
        {
            
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(TDatabase).Name}", new[] { DatabaseFolderPath });
            
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<TDatabase>(path);

                if (asset != null && asset.name == Database)
                {
                    _database = asset;
                    break;
                }
            }

            if (_database == null)
            {
                Debug.LogWarning($"Database not found: {Database} in folder {DatabaseFolderPath}");
            }
        }
    }
}
