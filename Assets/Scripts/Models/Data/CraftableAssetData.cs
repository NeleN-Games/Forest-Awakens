using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Models.Data
{
    public abstract class CraftableAssetData<TEnum> : CommonAssetData<TEnum>,ICraftable<BuildingType>
        where TEnum : System.Enum
    {
        public List<SourceRequirement> resourceRequirements;
        public CategoryType categoryType;
        public CraftableAvailabilityState craftableAvailabilityState;
        public UniqueId UniqueId { get; set; }
        
        public virtual UniqueId GetUniqueId()=>UniqueId;
        public void Initialize(GameObject prefab, Sprite icon, TEnum enumType,
            List<SourceRequirement> resourceRequirements,CategoryType categoryType, UniqueId uniqueId, CraftableAvailabilityState craftableAvailabilityState)
        {
            base.Initialize(prefab, icon, enumType);
            this.resourceRequirements =resourceRequirements;
            this.categoryType =categoryType;
            this.UniqueId =uniqueId;
            this.craftableAvailabilityState =craftableAvailabilityState;
        }

        protected override bool IsValid()
        {
            return base.IsValid() && resourceRequirements != null && resourceRequirements.Count > 0;
        }
        
    }
}
