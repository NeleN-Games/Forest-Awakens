using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public static class ModifyEnumUtility 
    {
        public static void AddItemTypeToEnum(string itemName,string enumPath,string enumName)
        {
            if (!File.Exists(enumPath))
            {
                Debug.LogError("Enum file not found."); return;
            }

            string[] lines = File.ReadAllLines(enumPath);
            if (lines.Any(l => l.Contains(itemName)))
            {
                Debug.LogWarning($"Enum already contains {itemName}");
                return;
            }

            int insertIndex = Array.FindIndex(lines, l => l.Contains($"enum {enumName}")) + 2;
            if (insertIndex < 2)
            {
                Debug.LogError($"{enumName} enum not found."); return;
            }

            var newLines = lines.ToList();
            newLines.Insert(insertIndex, $"        {itemName},");
            File.WriteAllLines(enumPath, newLines);
            AssetDatabase.Refresh();
        }
    }
}
