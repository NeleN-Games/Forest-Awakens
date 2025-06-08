using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public static class ModifyEnumUtility 
    {
        public static void AddObjectTypeToEnum(string itemName,string enumPath,string enumName)
        {
            var sanitizeName=SanitizeName(itemName);
            if (!File.Exists(enumPath))
            {
                Debug.LogError("Enum file not found."); return;
            }

            string[] lines = File.ReadAllLines(enumPath);
            if (lines.Any(l => l.Contains(sanitizeName)))
            {
                Debug.LogWarning($"Enum already contains {sanitizeName}");
                return;
            }
            int enumStartIndex = Array.FindIndex(lines, l => l.Contains($"enum {enumName}"));
            if (enumStartIndex == -1)
            {
                Debug.LogError($"{enumName} enum not found.");
                return;
            }

            
            int insertIndex = Array.FindIndex(lines, enumStartIndex, l => l.Trim().StartsWith("}"));
            if (insertIndex == -1)
            {
                Debug.LogError($"Could not find the end of enum {enumName}.");
                return;
            }
            
            var newLines = lines.ToList();
            newLines.Insert(insertIndex, $"        {sanitizeName},");
            File.WriteAllLines(enumPath, newLines);
            AssetDatabase.Refresh();
        }
        public static string SanitizeName(string objectName)
        {
            string clean = objectName.Replace(" ", "_")
                .Replace("-", "_")
                .Replace(".", "_");

            if (string.IsNullOrEmpty(clean))
                return "";

            clean = char.ToUpper(clean[0]) + clean.Substring(1);
            return clean;
        }
    }
}
