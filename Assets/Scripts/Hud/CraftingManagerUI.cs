using System;
using System.Collections.Generic;
using Enums;
using Hud.Slots;
using Interfaces;
using Models.Data;
using UnityEngine;

namespace Hud
{
    public class CraftingManagerUI : MonoBehaviour
    {
        
        private Dictionary<CategoryType, CategorySlotUI> _categorySlots = new();
        private Dictionary<CategoryType, List<ICraftable>> _availableCraftablesByCategory = new();
        
        [SerializeField] private CategorySlotUI categorySlotPrefab;
        [SerializeField] private CraftableSlotUI craftableSlotPrefab; 

        [SerializeField] private Transform categoriesSlotParent;
        [SerializeField] private Transform craftableSlotsContainer;
      
        private List<CraftableSlotUI> craftableSlots = new();
        private int _maxCraftableCount = 0;
        

        private void UpdateAvailableObjects(Dictionary<CategoryType, List<ICraftable>> availableCraftableObjects)
        {
            foreach (var kvp in availableCraftableObjects)
            {
                var category = kvp.Key;
                var craftableList = kvp.Value;
                _availableCraftablesByCategory[category] = craftableList;
                if (!_categorySlots.TryGetValue(category, out var slotUI))
                {
                    var go = Instantiate(categorySlotPrefab, categoriesSlotParent);
                    slotUI = go.GetComponent<CategorySlotUI>();
                    if (slotUI == null)
                    {
                        Debug.LogError("CategorySlotPrefab is missing CategorySlotUI component!");
                        continue;
                    }

                    _categorySlots[category] = slotUI;
                }
                slotUI.Setup(category);
                slotUI.OnClicked -= ShowCategoryCraftableObjects;
                slotUI.OnClicked += ShowCategoryCraftableObjects;
            }
        }

        private void ShowCategoryCraftableObjects(CategorySlotUI clickedCategorySlot)
        {
            var category = clickedCategorySlot.Category;
            if (!_availableCraftablesByCategory.TryGetValue(category, out var craftableList))
                return;

            int itemCount = craftableList.Count;

            if (itemCount > _maxCraftableCount)
            {
                int toCreate = itemCount - _maxCraftableCount;
                for (int i = 0; i < toCreate; i++)
                {
                    var go = Instantiate(craftableSlotPrefab.gameObject, craftableSlotsContainer);
                    var slot = go.GetComponent<CraftableSlotUI>();
                    craftableSlots.Add(slot);
                }
                _maxCraftableCount = itemCount;
            }

            // فعال سازی و مقداردهی اسلات‌ها
            for (int i = 0; i < _maxCraftableCount; i++)
            {
                if (i < itemCount)
                {
                    craftableSlots[i].gameObject.SetActive(true);
                   // craftableSlots[i].Setup(craftableList[i]);
                }
                else
                {
                    craftableSlots[i].gameObject.SetActive(false);
                }
            }
        }
    }
    
}
