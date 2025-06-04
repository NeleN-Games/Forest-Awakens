using UnityEditor;
using UnityEngine;

namespace Editor.Utilities
{
    public static class Drawer
    {
        public static void DrawSectionHeader(string sectionTitle, Color headerColor)
        {
            var rect = EditorGUILayout.GetControlRect(false, 25);
            EditorGUI.DrawRect(rect, headerColor);
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
            };
            GUI.Label(rect, sectionTitle, style);
        }
    }
}
