using System;
using Enums;
using Interfaces;
using UnityEngine;

namespace Models.Data
{
    [CreateAssetMenu(menuName = "Data/Building")]
    [Serializable]
    public class BuildingData : CraftableAssetData<BuildingType>, IIdentifiable<BuildingType>
    {
        public override BuildingType GetEnum() => enumType;
    }
}
