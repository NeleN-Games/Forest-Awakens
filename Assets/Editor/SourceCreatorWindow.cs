using System.IO;
using Base_Classes;
using Databases;
using Editor.CategoryTool;
using Enums;
using Models.Data;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    
    public class SourceCreatorWindow : GenericDatabaseEditorWindow<SourceData,SourceDatabase,SourceType>
    {
        protected override bool SetRequiresResourceRequirements()
        {
            // no need Requirements
            return false;
        }

        protected override string GetEditorName()
        {
            return "Source";
        }

        protected override string GetExpectedDatabasePath()
        {
            return "Assets/Resources/Databases";
        }

        protected override string GetExpectedPrefabsPath()
        {
            return "Assets/Prefabs/Sources";
        }

        protected override string GetExpectedDatabaseName()
        {
            return "Source Database";
        }
        protected override string GetExpectedDataPath()
        {
            return "Assets/Resources/Sources";
        }

        protected override string GetEnumPath()
        {
            return $"Assets/Scripts/Enums/{EnumName}.cs";
        }
        
        protected override string GetNameFieldLabel()
        {
            return "Source Name";
        }

    }
}

