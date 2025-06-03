using Databases;
using Enums;
using Models.Data;

namespace Editor
{
    public class BuildingCreatorWindow : GenericDatabaseEditorWindow<BuildingData,BuildingDatabase,BuildingType>
    {
        protected override bool SetRequiresResourceRequirements()
        {
            // need to set Requirements
            return true;
        }

        protected override string GetEditorName()
        {
            return "Building";
        }

        protected override string GetExpectedDatabasePath()
        {
            return "Assets/Resources/Databases";
        }
        
        protected override string GetExpectedPrefabsPath()
        {
            return "Assets/Prefabs/Buildings";
        }
        protected override string GetExpectedDatabaseName()
        {
            return "Building Database";
        }
        protected override string GetExpectedDataPath()
        {
            return "Assets/Resources/Buildings";
        }

        protected override string GetEnumPath()
        {
            return $"Assets/Scripts/Enums/{EnumName}.cs";
        }
        
        protected override string GetNameFieldLabel()
        {
            return "Building Name";
        }

    }
}
