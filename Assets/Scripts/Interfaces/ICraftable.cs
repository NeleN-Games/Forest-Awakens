using Models;

namespace Interfaces
{
    public interface ICraftable<out TEnum>
    {
        public UniqueId UniqueId { get; set; }
        public UniqueId GetUniqueId();
    }
}