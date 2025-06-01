using System;
using Enums;
using Interfaces;
using UnityEngine;

namespace Models.Data
{
    [CreateAssetMenu(menuName = "Data/Source")]
    [Serializable]
    public class SourceData : ScriptableObject, IIdentifiable<SourceType>
    {
        public SourceType type;
        public GameObject prefab;
        public Sprite icon;
        public SourceType GetID() => type;
        public bool IsValid()
        {
            return prefab != null 
                   && icon != null;
        }
    }
}
