using Databases;
using Enums;
using Models.Data;
using UnityEditor;

namespace Editor
{
    public class Itemgenereiccreator : GenericDatabaseEditorWindow<ItemData,ItemDatabase,ItemType>
    {
        protected override string FolderPath { get; }
        protected override string EnumName { get; }
        protected override string EnumPath { get; }
        protected override ItemDatabase GetDatabase()
        {
            return AssetDatabase.LoadAssetAtPath<ItemDatabase>("/Assets/Scriptable Objects/ItemDatabase.asset");
        }
     
        protected override void AddToDatabase(ItemData data)
        {
            GetDatabase().Entries.Add(data);
        }

        protected override void ClearDatabase()
        {
            GetDatabase().Entries.Clear();
        }

        protected override void DrawExtraFields(ItemData data)
        {
            // اگر فیلد اضافی دارید اینجا رسمش کنید
        }

        [MenuItem("Tools/Open " + nameof(Itemgenereiccreator) + " Window")]
        public static void OpenWindow()
        {
            GetWindow<Itemgenereiccreator>().Show();

        }
    }
}
