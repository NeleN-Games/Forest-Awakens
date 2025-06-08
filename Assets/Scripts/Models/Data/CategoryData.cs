using System.Collections.Generic;
using Attributes;
using Enums;
using UnityEngine;

namespace Models.Data
{
    [System.Serializable]
    public class CategoryData
    {
        [ReadOnly] public CategoryType type;
        [ReadOnly] public string name;
        public Sprite icon;
        [ReadOnly]public List<UniqueId> uniqueIds;
    }
}