using System;
using System.Collections.Generic;
using System.Linq;
using Databases;
using Enums;
using Hud.Slots;
using Interfaces;
using Models;
using Models.Data;
using Services;
using UnityEngine;

namespace Managers
{
    public class CraftLookupManager : MonoBehaviour,IInitializable
    {       
        
        /// <summary>
        /// Invoked when the availability of craftable objects changes, such as:
        /// - A new item is discovered in the tech tree.
        /// - The required resources for crafting become insufficient or sufficient.
        /// </summary>
        public Action<SourceType,int> OnSourceAmountChanged;

        /// <summary>
        /// Invoked when the availability of craftable objects changes, such as:
        /// - A new item is discovered in the tech tree.
        /// </summary>
        public Action<Dictionary<CategoryType, List<ICraftable>>> OnUnlockedCraftableObject;
        
        private readonly Dictionary<SourceType, List<ICraftable>> craftablesByRequiredSource;

        private readonly Dictionary<CategoryType, List<ICraftable>> _unlockedCraftable = new();
        
        private readonly Dictionary<ICraftable, CraftableSlotUI> CraftableSlotByICraftable;

        private IInventoryService _inventory;

        public void Initialize()
        {
            _inventory = ServiceLocator.Get<PlayerInventory>();
            if (_inventory == null)
                Debug.LogError("Inventory Service is not registered!");
            OnSourceAmountChanged += UpdateCraftableAvailability;
            OnUnlockedCraftableObject += UpdateUnlockedCraftableObjects;
            InitializeDatabases();
        }

        public void OnDestroy()
        {
            OnSourceAmountChanged -= UpdateCraftableAvailability;
            OnUnlockedCraftableObject -= UpdateUnlockedCraftableObjects;
        }

        private void InitializeDatabases()
        {
            var itemDatabase=ServiceLocator.Get<ItemDatabase>();
            var buildingDatabase=ServiceLocator.Get<BuildingDatabase>();
            
            AddUnlockedEntries<ItemType, ItemData>(itemDatabase.Entries);
            AddUnlockedEntries<BuildingType, BuildingData>(buildingDatabase.Entries);
           
        }
        private void AddUnlockedEntries<TEnum, TData>(List<TData> entries)
            where TEnum : Enum
            where TData : CraftableAssetData<TEnum>
        {

            foreach (var entry in entries)
            {
                if (entry.CraftableAvailabilityState == CraftableAvailabilityState.Locked)
                    continue;
               
                if (!_unlockedCraftable.ContainsKey(entry.CategoryType))
                {
                    _unlockedCraftable[entry.CategoryType] = new List<ICraftable>();
                }
                _unlockedCraftable[entry.CategoryType].Add((TData)entry.Clone());
                
                foreach (var sourceRequirement in entry.GetRequirements())
                {
                    SourceType sourceType = sourceRequirement.sourceType;

                    if (!craftablesByRequiredSource.ContainsKey(sourceType))
                    {
                        craftablesByRequiredSource[sourceType] = new List<ICraftable>();
                    }
                    craftablesByRequiredSource[sourceType].Add((TData)entry.Clone());
                }
                
            }
            
        }

        private void UpdateUnlockedCraftableObjects(Dictionary<CategoryType, List<ICraftable>> unlockedCraftableObjects)
        {
            foreach (var (categoryType, craftables) in unlockedCraftableObjects)
            {
                foreach (var craftable in craftables)
                {
                    if (craftable.CraftableAvailabilityState != CraftableAvailabilityState.Locked)
                    {
                        Debug.Log($"This Craftable: {craftable} is unlocked already");
                        continue;
                    }

                    if (!_unlockedCraftable.ContainsKey(categoryType))
                    {
                        _unlockedCraftable[categoryType] = new List<ICraftable>();
                        
                        // TODO : CREATE NEW CATEGORY SLOT
                    }
                    
                    _unlockedCraftable[categoryType].Add(craftable);
                    
                    // TODO : Check availability of craftable objects and set it .
                }
            }
        }

 
        private void UpdateCraftableAvailability(SourceType sourceType,int amount)
        {
            if (!craftablesByRequiredSource.TryGetValue(sourceType, out var affectedCraftables))
                return;

            foreach (var craftable in affectedCraftables)
            {         
                bool isAvailabilityChanged = craftable.IsAvailabilityChanged(_inventory);
                if (isAvailabilityChanged)
                {
                    ChangeCraftableSlotUIAvailability(craftable,craftable.CraftableAvailabilityState==CraftableAvailabilityState.Available);
                }
            }
        }

        private void ChangeCraftableSlotUIAvailability(ICraftable craftable, bool isAvailable)
        {
            if (CraftableSlotByICraftable.ContainsKey(craftable))
            {
                CraftableSlotByICraftable[craftable].ChangeAvailability(isAvailable);
            }
            else
            {
                Debug.LogError($"{nameof(craftable)} is not added to {nameof(CraftableSlotByICraftable)}");
            }
        }
        /*private void AddUnlockedEntries<TEnum, TData>(List<TData> entries)
            where TEnum : Enum
            where TData : CraftableAssetData<TEnum>,ICraftable
        {

            foreach (var entry in entries)
            {
                var uid = entry.GetUniqueId();
                
                if (uid != null && !_uniqueIdLookup.ContainsKey(uid))
                    _uniqueIdLookup[uid] = entry;

                if (!_categoryLookup.TryGetValue(entry.CategoryType, out var list))
                {
                    list = new List<ICraftable>();
                    _categoryLookup[entry.CategoryType] = list;
                }
                list.Add(entry);
            }

            UpdateAvailableCategoryLookup();
        }
        public ICraftable GetByUniqueId(UniqueId id) =>
            _uniqueIdLookup.GetValueOrDefault(id);

        public List<ICraftable> GetByCategory(CategoryType category) =>
            _categoryLookup.TryGetValue(category, out var list) ? list : new List<ICraftable>();

        private void UpdateAvailableCategoryLookup()
        {
            _availableCategoryLookup.Clear();

            foreach (var kvp in _categoryLookup)
            {
                var filteredList = kvp.Value
                    .FindAll(item => item.CraftableAvailabilityState  == CraftableAvailabilityState.Available
                                     || item.CraftableAvailabilityState == CraftableAvailabilityState.Unavailable);

                if (filteredList.Count > 0)
                    _availableCategoryLookup[kvp.Key] = filteredList;
            }
        }*/
    }
}
