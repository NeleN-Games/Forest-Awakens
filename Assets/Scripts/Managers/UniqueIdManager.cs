using System.Collections.Generic;
using System.IO;
using System.Linq;
using Databases;
using Interfaces;
using Models;
using Models.Data;
using UnityEditor;
using UnityEngine;

namespace Managers
{
    public static class UniqueIdManager
    {
        public static UniqueIdDatabase uniqueIdDatabase;
      
        public static UniqueId CreateNewUniqueId()
        {
            DatabasesManager.LoadDatabases();
            uniqueIdDatabase=DatabasesManager.uniqueIdDatabase;
            
            if (uniqueIdDatabase == null)
            {
                Debug.LogError("❌ UniqueIdDatabase not loaded. Call LoadDatabases() first.");
                return null;
            }

            const int min = 10000;
            const int max = 99999;
            HashSet<int> usedIds = new HashSet<int>(uniqueIdDatabase.uniqueIds.Select(u => u.id));

            int id;
            int maxTries = 1000;
            int attempts = 0;

            do
            {
                id = Random.Range(min, max + 1);
                attempts++;
                if (attempts > maxTries)
                {
                    Debug.LogError("❌ Failed to generate a unique ID after too many tries.");
                    return null;
                }
            }
            while (usedIds.Contains(id));

            UniqueId newUniqueId = new UniqueId { id = id };
            uniqueIdDatabase.uniqueIds.Add(newUniqueId);

#if UNITY_EDITOR
            EditorUtility.SetDirty(uniqueIdDatabase);
            AssetDatabase.SaveAssets();
#endif

            Debug.Log($"✅ New Unique ID generated: {newUniqueId}");
            return newUniqueId;
        }
        
        public static CraftableAssetData<System.Enum> FindCraftableByUniqueId(UniqueId id)
        {
            DatabasesManager.LoadDatabases();

            // Cast to base class with Enum
            var itemMatch = FindInDatabase<Enums.ItemType, ItemData>(DatabasesManager.itemDatabase, id);
            if (itemMatch != null)
                return itemMatch;

            var buildingMatch = FindInDatabase<Enums.BuildingType, BuildingData>(DatabasesManager.buildingDatabase, id);
            if (buildingMatch != null)
                return buildingMatch;

            Debug.LogWarning($"❓ No craftable asset found with UniqueId: {id.id}");
            return null;
        }
        private static CraftableAssetData<System.Enum> FindInDatabase<TEnum, TData>(GenericDatabase<TEnum, TData> database, UniqueId id)
            where TEnum : System.Enum
            where TData : CraftableAssetData<TEnum>, IIdentifiable<TEnum>
        {
            if (database == null) return null;

            foreach (var entry in database.Entries)
            {
                if (entry != null && entry.UniqueId != null && entry.UniqueId.id == id.id)
                    return entry as CraftableAssetData<System.Enum>;
            }

            return null;
        }
        
    }
}
