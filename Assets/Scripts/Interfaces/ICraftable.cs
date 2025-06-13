using System.Collections.Generic;
using Enums;
using Models;

namespace Interfaces
{
    public interface ICraftable
    {
        public UniqueId UniqueId { get; set; }
        
        UniqueId GetUniqueId();
        CategoryType CategoryType { get; set; }
        CraftableAvailabilityState CraftableAvailabilityState { get; set; }
        void Craft();
        public List<SourceRequirement> GetRequirements();
        public void SetRequirements(List<SourceRequirement> sourceRequirements);
        public bool IsAvailabilityChanged(IInventoryService inventory);

    }
}