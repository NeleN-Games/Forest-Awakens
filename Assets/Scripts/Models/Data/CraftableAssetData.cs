using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Models.Data
{
    public abstract class CraftableAssetData<TEnum> : CommonAssetData<TEnum>
        where TEnum : System.Enum
    {
        public List<SourceRequirement> resourceRequirements;
        public CategoryType categoryType;
        public UniqueId uniqueId;
        public CraftableAvailabilityState craftableAvailabilityState;
        public virtual UniqueId GetUniqueId()=>uniqueId;
        public void Initialize(GameObject prefab, Sprite icon, TEnum enumType,
            List<SourceRequirement> resourceRequirements,CategoryType categoryType, UniqueId uniqueId, CraftableAvailabilityState craftableAvailabilityState)
        {
            base.Initialize(prefab, icon, enumType);
            this.resourceRequirements =resourceRequirements;
            this.categoryType =categoryType;
            this.uniqueId =uniqueId;
            this.craftableAvailabilityState =craftableAvailabilityState;
        }

        protected override bool IsValid()
        {
            return base.IsValid() && resourceRequirements != null && resourceRequirements.Count > 0;
        }
    }
}
