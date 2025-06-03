using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Models;
using Models.Data;
using UnityEditor;
using UnityEngine;

namespace Editor.Utilities
{
    public static class ResourceRequirementDrawer
    {
        public static void Draw(ref List<SourceRequirement> requirements)
        {
            var allSourceTypes = Enum.GetValues(typeof(SourceType)).Cast<SourceType>();
            var selectedTypes = requirements.Select(r => r.sourceType).ToHashSet();
            bool canAdd = selectedTypes.Count < allSourceTypes.Count();

            EditorGUI.BeginDisabledGroup(!canAdd);
            if (GUILayout.Button(new GUIContent("Add Resource", EditorGUIUtility.IconContent("d_Toolbar Plus").image), GUILayout.Height(25)))
            {
                var remaining = allSourceTypes.Except(selectedTypes).ToList();
                if (remaining.Count > 0)
                    requirements.Add(new SourceRequirement { sourceType = remaining[0], amount = 1 });
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.BeginVertical("box");
            for (int i = 0; i < requirements.Count; i++)
            {
                DrawRow(requirements, i);
            }
            EditorGUILayout.EndVertical();
            
            SourceRequirementPrefs.Save(requirements);
        }

        private static void DrawRow(List<SourceRequirement> requirements, int i)
        {
            EditorGUILayout.BeginHorizontal();
            var otherTypes = requirements
                .Where((_, index) => index != i)
                .Select(r => r.sourceType)
                .ToHashSet();

            var available = Enum.GetValues(typeof(SourceType)).Cast<SourceType>()
                .Where(t => !otherTypes.Contains(t) || t == requirements[i].sourceType).ToList();

            int index = available.IndexOf(requirements[i].sourceType);
            int newIndex = EditorGUILayout.Popup(index, available.Select(t => t.ToString()).ToArray());
            if (newIndex >= 0)
            {
                requirements[i].sourceType = available[newIndex];
            }

            requirements[i].amount = EditorGUILayout.IntField(requirements[i].amount);

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                requirements.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
