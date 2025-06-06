using Enums;
using Helper;
using Models;
using UnityEngine;

namespace Managers
{
    public class CraftManager : MonoBehaviour
    {
        public BuildingCrafter buildingCrafter;
        public ItemCrafter itemCrafter;

        public void CraftBuilding(BuildingType type)
        {
            buildingCrafter.OnCraft?.Invoke(new CraftCommand<BuildingType>(type));
        }
        
        public void CraftItem(ItemType type)
        {
            itemCrafter.OnCraft?.Invoke(new CraftCommand<ItemType>(type));
        }
    }
}
