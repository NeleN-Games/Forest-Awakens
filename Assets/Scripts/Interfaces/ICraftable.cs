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
    }
}