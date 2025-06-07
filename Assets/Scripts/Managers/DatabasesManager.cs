using System.IO;
using Databases;
using Models.Scriptable_Objects;
using UnityEngine;

namespace Managers
{
    public class DatabasesManager : MonoBehaviour
    {
        public static UniqueIdDatabase uniqueIdDatabase;
        public static ItemDatabase itemDatabase;
        public static BuildingDatabase buildingDatabase;
        public static CategoryDatabase categoryDatabase;
        public static SourceDatabase sourceDatabase;

        public static void LoadDatabases()
        {
            itemDatabase = LoadDatabase<ItemDatabase>("Databases/Item Database");
            buildingDatabase = LoadDatabase<BuildingDatabase>("Databases/Building Database");
            sourceDatabase = LoadDatabase<SourceDatabase>("Databases/Source Database");
            categoryDatabase = LoadDatabase<CategoryDatabase>("Databases/Category Database");
            uniqueIdDatabase = LoadDatabase<UniqueIdDatabase>("Databases/Unique Id Database");
        }

        private static T LoadDatabase<T>(string path) where T : ScriptableObject
        {
            T db = Resources.Load<T>(path);
            if (db == null)
            {
                if (File.Exists(Path.Combine(Application.dataPath, "Resources", path + ".asset")))
                {
                    Debug.LogError(
                        $"⚠️ {typeof(T).Name}.asset exists but failed to load via Resources.Load. Ensure it's a ScriptableObject and inside a Resources folder.");
                }
                else
                {
                    Debug.LogWarning($"⚠️ {typeof(T).Name}.asset not found at path: {path}. Creating a new one.");
                }
            }
            else
            {
                Debug.Log($"✅ {typeof(T).Name} loaded successfully from Resources.");
            }

            return db;
        }
    }
}
