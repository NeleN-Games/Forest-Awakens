using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Editor
{
    public abstract class GenericDatabaseEditorWindow<TData, TDatabase, TEnum> : EditorWindow
        where TData : ScriptableObject, new()
        where TDatabase : ScriptableObject
        where TEnum : System.Enum
    {
        protected List<TData> dataList = new();
        protected TData selectedData;
        private UnityEditor.Editor selectedDataEditor;

        protected abstract string FolderPath { get; }
        protected abstract string EnumName { get; }
        protected abstract string EnumPath { get; }

        protected abstract TDatabase GetDatabase();
        protected abstract void AddToDatabase(TData data);
        protected abstract void ClearDatabase();
        protected abstract void DrawExtraFields(TData data);

       

        protected virtual void OnEnable()
        {
            LoadAssets();
        }

        private void LoadAssets()
        {
            dataList = AssetDatabase.FindAssets($"t:{typeof(TData).Name}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<TData>(AssetDatabase.GUIDToAssetPath(guid)))
                .OrderBy(asset => asset.name)
                .ToList();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            DrawList();
            DrawInspector();
            EditorGUILayout.EndHorizontal();

            DrawBottomButtons();
        }

        protected virtual void DrawList()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(200));
            foreach (var data in dataList)
            {
                if (GUILayout.Button(data.name))
                {
                    selectedData = data;
                    selectedDataEditor = UnityEditor.Editor.CreateEditor(selectedData);
                }
            }
            if (GUILayout.Button("+ Create New"))
            {
                CreateNewAsset();
            }
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawInspector()
        {
            if (selectedData == null) return;

            EditorGUILayout.BeginVertical();
            selectedDataEditor?.OnInspectorGUI();
            DrawExtraFields(selectedData);
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawBottomButtons()
        {
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Sync Enum"))
            {
                SyncEnum();
            }
            if (GUILayout.Button("Export to Database"))
            {
                ExportDatabase();
            }
            EditorGUILayout.EndHorizontal();
        }

        protected void CreateNewAsset()
        {
            var asset = ScriptableObject.CreateInstance<TData>();
            string path = $"{FolderPath}/{typeof(TData).Name}_{dataList.Count}.asset";
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            LoadAssets();
        }

        protected virtual void SyncEnum()
        {
            string[] names = dataList.Select(d => d.name.Replace(" ", "")).ToArray();
            EnumGenerator.GenerateEnum(EnumName, names, EnumPath);
            AssetDatabase.Refresh();
        }

        protected virtual void ExportDatabase()
        {
            var database = GetDatabase();
            ClearDatabase();

            foreach (var data in dataList)
                AddToDatabase(data);

            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log($"{typeof(TDatabase).Name} exported.");
        }
    }
}
