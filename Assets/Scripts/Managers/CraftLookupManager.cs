using System;
using System.Collections.Generic;
using Databases;
using Enums;
using Models;
using Models.Data;
using UnityEngine;

namespace Managers
{
    public class CraftLookupManager : MonoBehaviour
    {
        private readonly Dictionary<UniqueId, CraftableAssetData<Enum>> _uniqueIdLookup = new();
        private readonly Dictionary<CategoryType, List<CraftableAssetData<Enum>>> _categoryLookup = new();
        private readonly Dictionary<CategoryType, List<CraftableAssetData<Enum>>> _availableCategoryLookup = new();

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
        }

        private void AddEntries<TEnum, TData>(IEnumerable<TData> entries)
            where TEnum : Enum
            where TData : CraftableAssetData<TEnum>
        {
            foreach (var entry in entries)
            {
                var uid = entry.GetUniqueId();
                if (uid != null && !_uniqueIdLookup.ContainsKey(uid))
                    _uniqueIdLookup[uid] = entry as CraftableAssetData<Enum>;

                if (!_categoryLookup.TryGetValue(entry.categoryType, out var list))
                {
                    list = new List<CraftableAssetData<Enum>>();
                    _categoryLookup[entry.categoryType] = list;
                }
                list.Add(entry as CraftableAssetData<Enum>);
            }

            UpdateAvailableCategoryLookup();
        }
        public CraftableAssetData<Enum> GetByUniqueId(UniqueId id) =>
            _uniqueIdLookup.GetValueOrDefault(id);

        public List<CraftableAssetData<Enum>> GetByCategory(CategoryType category) =>
            _categoryLookup.TryGetValue(category, out var list) ? list : new List<CraftableAssetData<Enum>>();

        private void UpdateAvailableCategoryLookup()
        {
            _availableCategoryLookup.Clear();

            foreach (var kvp in _categoryLookup)
            {
                Debug.Log($"kvp{kvp.Key}, kvp values{kvp.Value.Count},kvp values: {kvp.Value[0]}, {kvp.Value[1]}");
                var filteredList = kvp.Value
                    .FindAll(item => item.craftableAvailabilityState == CraftableAvailabilityState.Available
                                     || item.craftableAvailabilityState == CraftableAvailabilityState.Unavailable);

                if (filteredList.Count > 0)
                    _availableCategoryLookup[kvp.Key] = filteredList;
            }
        }
    }
}
