using System;
using Enums;
using Interfaces;
using UnityEngine;

namespace Models.Data
{
    [CreateAssetMenu(menuName = "Crafting/Building")]
    [Serializable]
    public class BuildingData : CraftableData, ICraftable<BuildingType>, IIdentifiable<BuildingType>
    {

        public BuildingType type;

        public bool IsValid()
        {
            return prefab != null
                   && icon != null;
        }
        public BuildingType GetID() => type;
    }
}
