using Databases;
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

        private void Awake()
        {
            ServiceLocator.Register(sourceDatabase);
            ServiceLocator.Register(itemDatabase);
            ServiceLocator.Register(buildingDatabase);
            InitializeManagers();
        }

        private void InitializeManagers()
        {
            craftLookupManager.Initialize(ServiceLocator.Get<ItemDatabase>(),ServiceLocator.Get<BuildingDatabase>());
        }
    }
}