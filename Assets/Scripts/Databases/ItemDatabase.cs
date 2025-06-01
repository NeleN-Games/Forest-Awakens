using Enums;
using Models.Data;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Database/Item Database")]
    public class ItemDatabase : GenericDatabase<ItemType, ItemData>
    {
        
    }
}