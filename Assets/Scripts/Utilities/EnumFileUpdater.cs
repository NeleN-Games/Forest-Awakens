using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public class EnumFileUpdater
    { 
        private string _enumName;
        private string _enumFilePath;
        private List<string> _newMembers;

        public EnumFileUpdater(string enumName, string enumFilePath, IEnumerable<string> newMembers)
        {
            _enumName = enumName;
            _enumFilePath = enumFilePath;
            _newMembers = newMembers.ToList();
        }

        public bool UpdateEnumFile()
        {
            if (!File.Exists(_enumFilePath))
            {
                Debug.LogError($"Enum file not found at {_enumFilePath}");
                return false;
            }

            var lines = File.ReadAllLines(_enumFilePath).ToList();

            int enumStartIndex = lines.FindIndex(l => l.Contains($"enum {_enumName}"));
            if (enumStartIndex == -1)
            {
                Debug.LogError($"Enum {_enumName} not found in file.");
                return false;
            }

            int startInsertIndex = enumStartIndex + 2;

            int enumEndIndex = startInsertIndex;
            while (enumEndIndex < lines.Count && !lines[enumEndIndex].Trim().StartsWith("}"))
            {
                enumEndIndex++;
            }
            
            var enumLines = lines.GetRange(startInsertIndex, enumEndIndex - startInsertIndex);
            
            var currentEnumMembers = enumLines
                .Select(line => line.Trim().TrimEnd(',').Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            var newEnumMembers = new HashSet<string>(_newMembers);

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
                    enumEndIndex++;
                    modified = true;
                }
            }

            if (modified)
            {
                File.WriteAllLines(_enumFilePath, lines);
                AssetDatabase.Refresh();
                Debug.Log($"{_enumName} enum synced with database.");
                return true;
            }
            else
            {
                Debug.Log($"{_enumName} enum already synced.");
                return false;
            }
        } 
    }

}
