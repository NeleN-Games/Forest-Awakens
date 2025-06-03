using System;
using Enums;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class MainDatabaseEditorWindow : EditorWindow
    {
        private enum Tab { Source, Item, Building }
        private Tab _currentTab;

        private SourceCreatorWindow _sourceEditor;
        private ItemCreatorWindow _itemEditor;
        private BuildingCreatorWindow _buildingEditor;

        
        
        
        [MenuItem("Tools/Main Database Editor")]
        public static void Open()
        {
            GetWindow<MainDatabaseEditorWindow>("Database Editor");
        }

        private void OnEnable()
        {
            _sourceEditor = CreateInstance<SourceCreatorWindow>();
            _itemEditor = CreateInstance<ItemCreatorWindow>();
            _buildingEditor = CreateInstance<BuildingCreatorWindow>();
        }

        private void OnDestroy()
        {
            EditorPrefs.SetBool($"EnumReady_{nameof(ItemType)}", false);
            EditorPrefs.SetBool($"EnumReady_{nameof(SourceType)}", false);
            EditorPrefs.SetBool($"EnumReady_{nameof(BuildingType)}", false);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            _currentTab = (Tab)GUILayout.Toolbar((int)_currentTab, new[] { "Source", "Item", "Building" });
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool($"EnumReady_{nameof(ItemType)}", false);
                EditorPrefs.SetBool($"EnumReady_{nameof(SourceType)}", false);
                EditorPrefs.SetBool($"EnumReady_{nameof(BuildingType)}", false);
                
                EditorPrefs.SetString($"AssetName_{nameof(ItemType)}", "");
                EditorPrefs.SetString($"AssetName_{nameof(SourceType)}", "");
                EditorPrefs.SetString($"AssetName_{nameof(BuildingType)}", "");
                
            } 
            switch (_currentTab)
            {
                case Tab.Source:
                    _sourceEditor?.Draw();
                    break;
                case Tab.Item:
                    _itemEditor?.Draw();
                    break;
                case Tab.Building:
                    _buildingEditor?.Draw();
                    break;
            }
         
            //EditorGUILayout.EndVertical();
        }
    }
}