using System.Collections.Generic;
using UnityEngine;

namespace Models.Data
{
    [CreateAssetMenu(fileName = "CraftItem", menuName = "Crafting/Item")]
    public class CraftItemData : ScriptableObject
    {
        public string itemName;
        public string description;
        public Sprite displaySprite;
        public List<SourceRequirement> requiredResources = new List<SourceRequirement>();
    }
}