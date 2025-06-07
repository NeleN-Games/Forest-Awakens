using System;
using System.IO;
using System.Linq;
using Databases;
using Editor.CategoryTool;
using Editor.Utilities;
using Enums;
using Models.Data;
using Models.Scriptable_Objects;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Editor
{
    public class CategoryManagerWindow : EditorWindow
    {
        private CategoryDatabase categoryDatabase;
        private Vector2 scrollPos;
        private string newCategoryName = "";
        private Sprite newCategoryIcon;
        private bool isLoadedCategoryList = false;
        private bool hasUnsavedChanges = false;
        private GUIStyle redButtonStyle;
        private bool redButtonStyleInitialized = false;
        private const string CategoryPath = "Assets/Resources/Databases";
        private const string EnumName = "CategoryType";
        private const string EnumPath = "Assets/Scripts/Enums/CategoryType.cs";
        private const string DatabaseFileName = "Category Database";



        [MenuItem("Tools/Category Manager")]
        public static void ShowWindow()
        {
            GetWindow<CategoryManagerWindow>("Category Manager");
        }
        
        private void OnEnable()
        {
            LoadOrCreateCategoryList();
        }

        private void OnGUI()
        {
            if (!redButtonStyleInitialized)
            {
                redButtonStyle = new GUIStyle(GUI.skin.button);
                Texture2D redTexture = new Texture2D(1, 1);
                redTexture.SetPixel(0, 0, Color.red);
                redTexture.Apply();
                redButtonStyle.normal.background = redTexture;
                redButtonStyle.hover.background = redTexture;
                redButtonStyle.active.background = redTexture;

                redButtonStyleInitialized = true;
            }
            Drawer.DrawSectionHeader($"Category Manager", new Color(0.2f, 0.5f, 0.8f, 1f));

            EditorGUILayout.LabelField("Category  Asset", EditorStyles.boldLabel);


            if (categoryDatabase == null)
            {
                EditorGUILayout.HelpBox($"❌Category Database.asset not found at path: {CategoryPath} Database", MessageType.Warning);

                if (GUILayout.Button("Try Load Again"))
                {
                    LoadOrCreateCategoryList();
                }
                return;
            }
            EditorGUILayout.BeginVertical("box");
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.LabelField("Categories", EditorStyles.boldLabel);

            for (int i = 0; i < categoryDatabase.categories.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                categoryDatabase.categories[i].icon = (Sprite)EditorGUILayout.ObjectField(categoryDatabase.categories[i].icon, typeof(Sprite), false, GUILayout.Width(50), GUILayout.Height(50));
                if (GUI.changed) hasUnsavedChanges = true;
                
                categoryDatabase.categories[i].name = EditorGUILayout.TextField(categoryDatabase.categories[i].name);
                if (GUI.changed) hasUnsavedChanges = true;
                
                if (GUILayout.Button("↑", GUILayout.Width(25)) && i > 0)
                {
                    Swap(i, i - 1);
                    hasUnsavedChanges = true;
                }
                if (GUILayout.Button("↓", GUILayout.Width(25)) && i < categoryDatabase.categories.Count - 1)
                {
                    Swap(i, i + 1);
                    hasUnsavedChanges = true;
                }
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    categoryDatabase.categories.RemoveAt(i);
                    hasUnsavedChanges = true;
                }

                EditorGUILayout.EndHorizontal();

            }
            EditorGUILayout.EndScrollView();  // ✅ پایان ScrollView برای لیست

            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.BeginVertical("box");

            Drawer.DrawSectionHeader($"Category Configuration", new Color(0.1f, 0.5f, 0.1f, 1f));

            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Add New Category", EditorStyles.boldLabel);
            
            newCategoryName = EditorGUILayout.TextField("Name", newCategoryName);
            newCategoryIcon = (Sprite)EditorGUILayout.ObjectField("Icon", newCategoryIcon, typeof(Sprite), false);

            EditorGUI.BeginDisabledGroup(isLoadedCategoryList);
            categoryDatabase = (CategoryDatabase)EditorGUILayout.ObjectField("Loaded Category List", categoryDatabase, typeof(ScriptableObject), false);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(newCategoryName) || newCategoryIcon==null);
            if (GUILayout.Button("Add Category",GUILayout.Height(35)))
            {
                
                if (string.IsNullOrWhiteSpace(newCategoryName))
                {
                    EditorUtility.DisplayDialog("Error", "The category name can not be empty", "Ok");
                }
                else
                {
                    OnlineSpellChecker.CheckSpelling(newCategoryName.Trim(), correctedName =>
                    {
                        if (correctedName != newCategoryName.Trim())
                        {
                            Debug.Log($"🔧Correcting name: '{newCategoryName}' → '{correctedName}'");
                        }

                        bool exists = categoryDatabase.categories.Exists(c => c.name == correctedName);
                        if (exists)
                        {
                            EditorUtility.DisplayDialog("Error", "This name is used before", "Ok");
                        }
                        else
                        {
                            categoryDatabase.categories.Add(new CategoryData
                            {
                                name = correctedName,
                                icon = newCategoryIcon
                            });
                            SyncCategoryType();
                            EditorUtility.SetDirty(categoryDatabase);
                            AssetDatabase.SaveAssets();
                            newCategoryName = "";
                            newCategoryIcon = null;
                            EditorApplication.delayCall += () => Repaint();
                        }
                    });
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(20);
            if (hasUnsavedChanges)
            {
                if (GUILayout.Button("Save", redButtonStyle,GUILayout.Height(35)))
                {
                    SaveCategoryList();
                }
            }
            else
            {
                if (GUILayout.Button("Save",GUILayout.Height(35)))
                {
                    SaveCategoryList();
                }
            }
            EditorGUILayout.Space(40);

        }

        private void Swap(int indexA, int indexB)
        {
            (categoryDatabase.categories[indexA], categoryDatabase.categories[indexB]) = (categoryDatabase.categories[indexB], categoryDatabase.categories[indexA]);
            EditorUtility.SetDirty(categoryDatabase);
            AssetDatabase.SaveAssets();
        }

        private void LoadOrCreateCategoryList()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(CategoryDatabase)}", new[] { CategoryPath });
        
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<CategoryDatabase>(path);

                if (asset != null && asset.name == DatabaseFileName)
                {
                    categoryDatabase = asset;
                    break;
                }
                
            }

            if (categoryDatabase == null)
            {
                Debug.LogWarning($"Database not found: CategoryDatabase in folder {CategoryPath}");
                isLoadedCategoryList = false;
                return;
            }
            isLoadedCategoryList = true;
        }

        private void SaveCategoryList()
        {
            EditorUtility.SetDirty(categoryDatabase);
            SyncCategoryType();
            ModifyEnumUtility.AddItemTypeToEnum(newCategoryName,EnumPath,EnumName);
            AssetDatabase.SaveAssets();
            hasUnsavedChanges=false;
        }
        private void SyncCategoryType()
        {
            if (categoryDatabase == null)
            {
                Debug.LogWarning("Database is null, cannot sync enum.");
                return;
            }
            EnumSyncUtility.SyncCategoryEnumFromDatabase(EnumName, EnumPath, categoryDatabase);

            AssetDatabase.Refresh();
         
        }
        private string SanitizeName(string name)
        {
            string clean = name.Replace(" ", "").Replace("-", "").Replace(".", "");
            clean = char.ToUpper(clean[0]) + clean.Substring(1);
            return clean;
        }
    }
}
