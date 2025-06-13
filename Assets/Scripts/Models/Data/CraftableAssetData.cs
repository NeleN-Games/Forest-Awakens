using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Models.Data
{
    public abstract class CraftableAssetData<TEnum> : CommonAssetData<TEnum>,ICraftable
        where TEnum : System.Enum
    {
        public UniqueId UniqueId { get; set; }
        
        public virtual UniqueId GetUniqueId()=>UniqueId;
        public CategoryType CategoryType { get; set; }
        public CraftableAvailabilityState CraftableAvailabilityState { get; set; }
        public abstract void Craft();

        private List<SourceRequirement> _resourceRequirements;
        public List<SourceRequirement> GetRequirements() => _resourceRequirements;
        public void SetRequirements(List<SourceRequirement> sourceRequirements)
        {
            _resourceRequirements=sourceRequirements;
        }

        public bool IsAvailabilityChanged(IInventoryService inventory)
        {
            var oldState = CraftableAvailabilityState;
            var newState = CraftableAvailabilityState.Available;

            foreach (var req in _resourceRequirements)
            {
                int availableAmount = inventory.GetSourceAmount(req.sourceType);
                if (availableAmount < req.amount)
                {
                    newState = CraftableAvailabilityState.Unavailable;
                    break;
                }
            }

            CraftableAvailabilityState = newState;
            return oldState != newState;
        }
     

        public void Initialize(GameObject prefab, Sprite icon, TEnum enumType,
            List<SourceRequirement> resourceRequirements,CategoryType categoryType, UniqueId uniqueId, CraftableAvailabilityState craftableAvailabilityState)
        {
            base.Initialize(prefab, icon, enumType);
            this._resourceRequirements =resourceRequirements;
            this.CategoryType =categoryType;
            this.UniqueId =uniqueId;
            this.CraftableAvailabilityState =craftableAvailabilityState;
        }

        protected override bool IsValid()
        {
            return base.IsValid() && _resourceRequirements != null && _resourceRequirements.Count > 0;
        }
        
    }
}
