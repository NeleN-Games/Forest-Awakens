using System;
using Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Models.Data
{
    [Serializable]
    public class ItemData
    {
        public ItemType type;
        public GameObject prefab;
        public string itemName;
        public Sprite icon;
        public Color color;
        public bool IsValid()
        {
            return prefab != null 
                   && !string.IsNullOrEmpty(itemName) 
                   && icon != null;
        }
    }
}