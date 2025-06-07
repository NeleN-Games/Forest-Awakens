using System;
using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Models.Data
{
    [System.Serializable]
    public class CategoryData
    {
        public CategoryType type;
        public string name;
        public Sprite icon;
        public List<UniqueId> craftableAssets;
    }
}