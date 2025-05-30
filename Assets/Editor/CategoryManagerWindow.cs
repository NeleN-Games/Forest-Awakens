using System.IO;
using Editor.CategoryTool;
using Models.Data;
using Models.Scriptable_Objects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CategoryManagerWindow : EditorWindow
    {
        private CategoryList categoryList;
        private Vector2 scrollPos;
        private string newCategoryName = "";
        private Sprite newCategoryIcon;

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
            if (categoryList == null) return;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.LabelField("Categories", EditorStyles.boldLabel);

            for (int i = 0; i < categoryList.categories.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                // فیلد عکس برای هر کتگوری
                categoryList.categories[i].icon = (Sprite)EditorGUILayout.ObjectField(categoryList.categories[i].icon, typeof(Sprite), false, GUILayout.Width(50), GUILayout.Height(50));

                // فیلد نام کتگوری
                categoryList.categories[i].name = EditorGUILayout.TextField(categoryList.categories[i].name);

                if (GUILayout.Button("↑", GUILayout.Width(25)) && i > 0)
                {
                    Swap(i, i - 1);
                }
                if (GUILayout.Button("↓", GUILayout.Width(25)) && i < categoryList.categories.Count - 1)
                {
                    Swap(i, i + 1);
                }
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    categoryList.categories.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Add New Category", EditorStyles.boldLabel);
            
            newCategoryName = EditorGUILayout.TextField("Name", newCategoryName);

            if (GUILayout.Button("Add Category"))
            {
                
                if (string.IsNullOrWhiteSpace(newCategoryName))
                {
                    EditorUtility.DisplayDialog("Error", "The category name can not be empty", "Ok");
                }
                else
                {
                    // صبر می‌کنیم تا اصلاح تایپ آنلاین انجام بشه
                    OnlineSpellChecker.CheckSpelling(newCategoryName.Trim(), correctedName =>
                    {
                        // اگر اسم اصلاح شده متفاوت بود، پیام میدیم
                        if (correctedName != newCategoryName.Trim())
                        {
                            Debug.Log($"🔧Correcting name: '{newCategoryName}' → '{correctedName}'");
                        }

                        // چک برای اسم تکراری با اسم اصلاح شده
                        bool exists = categoryList.categories.Exists(c => c.name == correctedName);
                        if (exists)
                        {
                            EditorUtility.DisplayDialog("Error", "This name is used before", "Ok");
                        }
                        else
                        {
                            categoryList.categories.Add(new CategoryData
                            {
                                name = correctedName,
                                icon = newCategoryIcon
                            });
                            newCategoryName = "";
                            newCategoryIcon = null;

                            EditorUtility.SetDirty(categoryList);
                            AssetDatabase.SaveAssets();

                            // چون داخل callback هستیم، باید از این تابع استفاده کنیم تا UI رفرش بشه
                            EditorApplication.delayCall += () => Repaint();
                        }
                    });
                }
            }

            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(categoryList);
                AssetDatabase.SaveAssets();
            }

            if (GUILayout.Button("Generate CategoryType.cs"))
            {
                GenerateCategoryTypeFile();
            }

            EditorGUILayout.EndScrollView();
        }

        private void Swap(int indexA, int indexB)
        {
            (categoryList.categories[indexA], categoryList.categories[indexB]) = (categoryList.categories[indexB], categoryList.categories[indexA]);
            EditorUtility.SetDirty(categoryList);
            AssetDatabase.SaveAssets();
        }

        private void LoadOrCreateCategoryList()
        {
            categoryList = Resources.Load<CategoryList>("CategoryList");
            if (categoryList == null)
            {
                categoryList = CreateInstance<CategoryList>();
                Directory.CreateDirectory("Assets/Resources");
                AssetDatabase.CreateAsset(categoryList, "Assets/Resources/CategoryList.asset");
                AssetDatabase.SaveAssets();
                Debug.Log("Created new CategoryList.asset in Resources.");
            }
        }

        private void GenerateCategoryTypeFile()
        {
            if (categoryList == null) return;

            string path = "Assets/Scripts/Enums/CategoryType.cs";
            Directory.CreateDirectory("Assets/Scripts/Enums");

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("// Auto-generated by CategoryManagerWindow");
                writer.WriteLine("// Do not edit this file manually.");
                writer.WriteLine("namespace Enums");
                writer.WriteLine("{");
                writer.WriteLine("public enum CategoryType");
                writer.WriteLine("{");

                for (int i = 0; i < categoryList.categories.Count; i++)
                {
                    string safeName = SanitizeName(categoryList.categories[i].name);
                    if (string.IsNullOrWhiteSpace(safeName)) continue;

                    writer.Write($"    {safeName}");
                    if (i < categoryList.categories.Count - 1)
                        writer.WriteLine(",");
                    else
                        writer.WriteLine();
                }

                writer.WriteLine("}");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
            Debug.Log("✅ CategoryType enum generated successfully.");
        }

        private string SanitizeName(string name)
        {
            string clean = name.Replace(" ", "").Replace("-", "").Replace(".", "");
            clean = char.ToUpper(clean[0]) + clean.Substring(1);
            return clean;
        }
    }
}
