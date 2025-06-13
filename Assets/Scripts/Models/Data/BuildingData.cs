using System;
using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;

namespace Models.Data
{
    [CreateAssetMenu(menuName = "Data/Building")]
    [Serializable]
    public class BuildingData : CraftableAssetData<BuildingType>
    {
        public override BuildingType GetEnum() => enumType;
        public override CommonAssetData<BuildingType> Clone()
        {
            var clone = Instantiate(this);
            Debug.Log(clone.CategoryType);
            if (clone is CraftableAssetData<BuildingType> craftableClone)
            {
                PrintDetails();
            }
            else
            {
                Debug.Log("Clone created but it's not CraftableAssetData");
            }
            return clone;
        }


        public override void Craft()
        {
            Debug.Log("Crafting Building");
        }
        public void PrintDetails()
        {
            Debug.Log($"CategoryType: {CategoryType}");
            Debug.Log($"UniqueId: {UniqueId?.ID}");
            Debug.Log($"CraftableAvailabilityState: {CraftableAvailabilityState}");
            Debug.Log($"Resource Requirements Count: {GetRequirements()?.Count}");
            // میتونی موارد بیشتری هم اضافه کنی
        }
    }
}
