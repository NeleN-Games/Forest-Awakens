using System;
using Enums;
using Interfaces;
using UnityEngine;

namespace Models.Data
{
    [CreateAssetMenu(menuName = "Crafting/Item")]
    [Serializable]
    public class ItemData : CraftableData, ICraftable<ItemType>, IIdentifiable<ItemType>
    {
        public ItemType type;
        public bool IsValid()
        {
            return prefab != null 
                   && icon != null;
        }
        public ItemType GetID() => type;
    }
}