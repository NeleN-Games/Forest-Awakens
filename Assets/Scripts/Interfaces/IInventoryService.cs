using Enums;

namespace Interfaces
{
    public interface IInventoryService
    {
        int GetSourceAmount(SourceType sourceType);
    }
}