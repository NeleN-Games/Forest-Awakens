using Databases;
using Helper;
using Hud;
using Services;
using UnityEngine;

namespace Managers
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private SourceDatabase sourceDatabase;
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private BuildingDatabase buildingDatabase;
        [SerializeField] private CraftLookupManager craftLookupManager;
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private ItemCrafter itemCrafter;
        [SerializeField] private BuildingCrafter buildingCrafter;

        private void Awake()
        {
            ServiceLocator.Register(sourceDatabase);
            ServiceLocator.Register(itemDatabase);
            ServiceLocator.Register(buildingDatabase);
            ServiceLocator.Register(inventoryUI);
            ServiceLocator.Register(craftLookupManager);
            ServiceLocator.Register(playerInventory);
            ServiceLocator.Register(itemCrafter);
            ServiceLocator.Register(buildingCrafter);
            
            ServiceLocator.InitializeAll();
        }
    }
}