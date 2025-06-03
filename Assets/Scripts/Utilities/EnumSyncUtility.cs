using System;
using System.Collections.Generic;
using System.IO;
using Databases;
using Interfaces;
using Models.Data;
using UnityEngine;

namespace Utilities
{
    public static class EnumSyncUtility
    {
        public static void
            SyncEnumFromDatabase<TEnum, TData,TDatabase>(string enumName, string enumPath, TDatabase database)
            where TEnum : Enum
            where TData :  CommonAssetData<TEnum>, IIdentifiable<TEnum>
            where TDatabase : GenericDatabase<TEnum, TData>
        {
            List<string> itemNamesInDb = GetAllItemNamesFromDatabase<TEnum, TData,TDatabase>(database);
            var updater = new EnumFileUpdater(enumName, enumPath, itemNamesInDb);
            updater.UpdateEnumFile();
        }
        private static List<string> GetAllItemNamesFromDatabase<TEnum, TData,TDatabase>(TDatabase database)
            where TEnum : Enum
            where TData :  CommonAssetData<TEnum>, IIdentifiable<TEnum>
            where TDatabase : GenericDatabase<TEnum, TData>
        {
               
            if (database == null)
            {
                Debug.LogWarning($"{nameof(database)} is null.");
                return new List<string>();
            }

            if (database.Entries == null || database.Entries.Count == 0)
            {
                Debug.LogWarning($"{nameof(database)}.Entries is empty.");
                return new List<string>();
            }
            List<string> itemNames = new List<string>();

            foreach (var item in database.Entries)
            {
                if (!string.IsNullOrEmpty(item.name))
                    itemNames.Add(StringUtility.CapitalizeFirstLetter(item.name)); 
            }

            return itemNames;
        }
       

    }
}
