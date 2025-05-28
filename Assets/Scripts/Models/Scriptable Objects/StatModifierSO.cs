using System;
using System.Collections.Generic;
using Base_Classes;
using UnityEngine;

namespace Models.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "NewStatModifier", menuName = "Stats/StatModifier")]
    public class StatModifierSo : ScriptableObject
    {
        public string modifierName;
        public Sprite icon;
        public List<StatEffect> effects = new List<StatEffect>();
    }
}