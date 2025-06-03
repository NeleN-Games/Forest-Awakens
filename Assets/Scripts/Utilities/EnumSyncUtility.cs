using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Databases;
using Interfaces;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public static class EnumSyncUtility
    {
        public static void SyncEnumFromDatabase<TEnum, TData>(string enumName, string enumPath, List<string> namesInDb,
            GenericDatabase<TEnum, TData> database)
            where TEnum : Enum
            where TData : ScriptableObject, IIdentifiable<TEnum>
        {
            if (namesInDb == null)
            {
                Debug.LogWarning($"{nameof(namesInDb)} is null. Cannot sync enum.");
                return;
            }
            List<string> itemNamesInDb = GetAllItemNamesFromDatabase<TEnum, TData>(database);
            
            if (!File.Exists(enumPath))
            {
                Debug.LogError("Enum file not found.");
                return;
            }

            var lines = File.ReadAllLines(enumPath).ToList();

            int enumStartIndex = lines.FindIndex(l => l.Contains($"enum {nameof(enumName)}"));
            if (enumStartIndex == -1)
            {
                Debug.LogError($"{nameof(enumName)} enum not found.");
                return;
            }

            int startInsertIndex = enumStartIndex + 2;

            var enumLines = new List<string>();
            int i = startInsertIndex;
            while (i < lines.Count && !lines[i].Trim().StartsWith("}"))
            {
                enumLines.Add(lines[i]);
                i++;
            }

            var currentEnumMembers = enumLines
                .Select(line => line.Trim().TrimEnd(',').Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            var newEnumMembers = new HashSet<string>(itemNamesInDb);

            bool modified = false;

            for (int j = enumLines.Count - 1; j >= 0; j--)
            {
                var memberName = enumLines[j].Trim().TrimEnd(',').Trim();
                if (!newEnumMembers.Contains(memberName))
                {
                    lines.RemoveAt(startInsertIndex + j);
                    modified = true;
                }
            }

            currentEnumMembers = lines.Skip(startInsertIndex).TakeWhile(l => !l.Trim().StartsWith("}"))
                .Select(l => l.Trim().TrimEnd(',').Trim()).ToList();

            foreach (var itemName in newEnumMembers)
            {
                if (!currentEnumMembers.Contains(itemName))
                {
                    lines.Insert(startInsertIndex, $"        {itemName},");
                    startInsertIndex++;
                    modified = true;
                }
            }

            if (modified)
            {
                File.WriteAllLines(enumPath, lines);
                AssetDatabase.Refresh();
                Debug.Log($"{nameof(TEnum)} enum synced with database.");
            }
            else
            {
                Debug.Log($"{nameof(TEnum)} enum already synced.");
            }
        }
        private static List<string> GetAllItemNamesFromDatabase<TEnum, TData>(GenericDatabase<TEnum, TData> database)
            where TEnum : Enum
            where TData : ScriptableObject, IIdentifiable<TEnum>
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
                    itemNames.Add(CapitalizeFirstLetter(item.name)); 
            }

            return itemNames;
        }
        private static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            if (input.Length == 1)
                return input.ToUpper();
            return char.ToUpper(input[0]) + input.Substring(1);
        }

    }
}
