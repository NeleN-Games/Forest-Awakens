using System.Collections.Generic;
using Enums;

namespace Helper
{
    public static class UniqueIdRanges
    {
        public static readonly Dictionary<CraftableType, (int start, int end)> Ranges = new()
        {
            { CraftableType.Item,     (10000, 19999) },
            { CraftableType.Building, (20000, 29999) },
           
        };
    }
}