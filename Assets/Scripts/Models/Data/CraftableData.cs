using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Models.Data
{
    public abstract class CraftableData : ScriptableObject
    {
        public string description;
        public GameObject prefab;
        public List<SourceRequirement> requirements;
        public Sprite icon;
    }
}
