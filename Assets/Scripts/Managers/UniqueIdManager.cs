using System.Collections.Generic;
using System.IO;
using System.Linq;
using Databases;
using Enums;
using Helper;
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
      
        public static UniqueId CreateNewUniqueId(string uniqueName, CraftableType craftableType)
        {
            DatabasesManager.LoadDatabases();
            uniqueIdDatabase=DatabasesManager.uniqueIdDatabase;
            
            if (uniqueIdDatabase == null)
            {
                Debug.LogError("❌ UniqueIdDatabase not loaded. Call LoadDatabases() first.");
                return null;
            }
            if (!UniqueIdRanges.Ranges.TryGetValue(craftableType, out var range))
            {
                Debug.LogError($"❌ Unknown CraftableType {craftableType}");
                return null;
            }

           
            int min = range.start;
            int max = range.end;
            
            
            HashSet<int> usedIds = new HashSet<int>(
                uniqueIdDatabase.uniqueIds
                    .Where(u => u.GetCraftableType() == craftableType)
                    .Select(u => u.ID)
            );
            
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

            UniqueId newUniqueId = new UniqueId(uniqueName, id);
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
    
            var type = id.GetCraftableType();
            switch (type)
            {
                case CraftableType.Item:
                    return FindInDatabase<Enums.ItemType, ItemData>(DatabasesManager.itemDatabase, id);
                case CraftableType.Building:
                    return FindInDatabase<Enums.BuildingType, BuildingData>(DatabasesManager.buildingDatabase, id);
                default:
                    Debug.LogError($"Craftable type not supported for ID: {id.ID}");
                    return null;
            }
           
        }
        private static CraftableAssetData<System.Enum> FindInDatabase<TEnum, TData>(GenericDatabase<TEnum, TData> database, UniqueId id)
            where TEnum : System.Enum
            where TData : CraftableAssetData<TEnum>
        {
            if (database == null) return null;

            foreach (var entry in database.Entries)
            {
                if (entry != null && entry.UniqueId != null && entry.UniqueId.ID == id.ID)
                    return entry as CraftableAssetData<System.Enum>;
            }

            return null;
        }
        
    }
}
