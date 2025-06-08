using System;
using System.Collections.Generic;
using Databases;
using Enums;
using Hud.Slots;
using Interfaces;
using Models;
using Models.Data;
using UnityEngine;

namespace Managers
{
    public class CraftLookupManager : MonoBehaviour
    {
        private readonly Dictionary<UniqueId, ICraftable> _uniqueIdLookup = new();
        private readonly Dictionary<CategoryType, List<ICraftable>> _categoryLookup = new();
        private readonly Dictionary<CategoryType, List<ICraftable>> _availableCategoryLookup = new();

        public static CraftLookupManager instance;
        private void Awake()
        {
            instance ??= this;
        }

        public void Initialize(GenericDatabase<ItemType, ItemData> itemDatabase,
            GenericDatabase<BuildingType, BuildingData> buildingDatabase)
        {
            _uniqueIdLookup.Clear();
            _categoryLookup.Clear();

            AddEntries<ItemType, ItemData>(itemDatabase.Entries);
            AddEntries<BuildingType, BuildingData>(buildingDatabase.Entries);
            Debug.Log($"UniqueId Count: {_uniqueIdLookup.Count}");
            Debug.Log($"Category Count: {_categoryLookup.Count}");
        }

        private void AddEntries<TEnum, TData>(List<TData> entries)
            where TEnum : Enum
            where TData : CraftableAssetData<TEnum>,ICraftable
        {
            Debug.Log($"Added {entries.Count} entries of type {typeof(TData).Name}");

            foreach (var entry in entries)
            {
                Debug.Log($"Entry: {entry.name}, UID: {entry.GetUniqueId().UniqueName}, UID, Category: {entry.CategoryType}");

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
                Debug.Log($"kvp{kvp.Key}, kvp values{kvp.Value.Count},kvp values: {kvp.Value[0]}, {kvp.Value[1]}");
                var filteredList = kvp.Value
                    .FindAll(item => item.CraftableAvailabilityState  == CraftableAvailabilityState.Available
                                     || item.CraftableAvailabilityState == CraftableAvailabilityState.Unavailable);

                if (filteredList.Count > 0)
                    _availableCategoryLookup[kvp.Key] = filteredList;
            }
        }
    }
}
