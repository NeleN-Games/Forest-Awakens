using Models.Scriptable_Objects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(StatEffect))]
    public class StatEffectDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float lineHeight = EditorGUIUtility.singleLineHeight + 2f;
            Rect statTypeRect = new Rect(position.x, position.y, position.width, lineHeight);
            Rect amountRect = new Rect(position.x, position.y + lineHeight, position.width, lineHeight);
            Rect affectsRateRect = new Rect(position.x, position.y + lineHeight * 2, position.width, lineHeight);

            EditorGUI.PropertyField(statTypeRect, property.FindPropertyRelative("statType"));
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"));
            var affectsRateProp = property.FindPropertyRelative("hasEffectRate");
            EditorGUI.PropertyField(affectsRateRect, affectsRateProp);

            if (affectsRateProp.boolValue)
            {
                Rect rateAmountRect = new Rect(position.x, position.y + lineHeight * 3, position.width, lineHeight);
                Rect durationRect = new Rect(position.x, position.y + lineHeight * 4, position.width, lineHeight);
                EditorGUI.PropertyField(rateAmountRect, property.FindPropertyRelative("rateAmount"));
                EditorGUI.PropertyField(durationRect, property.FindPropertyRelative("duration"));
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var affectsRate = property.FindPropertyRelative("hasEffectRate").boolValue;
            int lines = affectsRate ? 5 : 3;
            return lines * (EditorGUIUtility.singleLineHeight + 2f);
        }
    }
}