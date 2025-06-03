using System;
using System.Collections.Generic;
using System.Text;
using Enums;
using Models;
using UnityEditor;

namespace Editor.Utilities
{
    public static class SourceRequirementPrefs
    {
        public const string PrefKey = "CashedSourceRequirements";
        
        public static void Save(List<SourceRequirement> requirements)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < requirements.Count; i++)
            {
                sb.Append($"{requirements[i].sourceType}_{requirements[i].amount}");
                if (i < requirements.Count - 1)
                    sb.Append("_");
            }

            EditorPrefs.SetString(PrefKey, sb.ToString());
        }
        public static List<SourceRequirement> Load()
        {
            var list = new List<SourceRequirement>();
            var data = EditorPrefs.GetString(PrefKey, "");

            if (string.IsNullOrEmpty(data))
                return list;

            var parts = data.Split('_');
            for (int i = 0; i < parts.Length - 1; i += 2)
            {
                if (Enum.TryParse(parts[i], out SourceType type) && int.TryParse(parts[i + 1], out int amount))
                {
                    list.Add(new SourceRequirement
                    {
                        sourceType = type,
                        amount = amount
                    });
                }
            }

            return list;
        }
        public static void RemoveAll()
        {
            EditorPrefs.DeleteKey(PrefKey);
        }
    }
}