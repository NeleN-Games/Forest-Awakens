using System;
using System.Collections.Generic;
using Enums;
using Models;
using Models.Data;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(fileName = "CategoryDatabase", menuName = "Tools/Category Database", order = 1)]
    public class CategoryDatabase : ScriptableObject
    {
        public List<CategoryData> categories = new List<CategoryData>();

        public void AddCraftableObjectToCategory(CategoryType categoryType,UniqueId uniqueId)
        {
            foreach (var category in categories)
            {
                Debug.Log(category.type);
                if (category.type != categoryType)
                {
                    Debug.LogError($"Did not find valid category to add category.type : {category.type},   categoryType : {categoryType}");
                    continue;
                }
              
                if (!category.uniqueIds.Contains(uniqueId))
                    category.uniqueIds.Add(uniqueId);
                
                return;
            }
            Debug.LogError($"Did not find valid category to add categoryType : {categoryType}");
        }

        public void RemoveCraftableObjectFromCategory(CategoryType categoryType, UniqueId uniqueId)
        {
            foreach (var category in categories)
            {
                Debug.Log(category.type);
                if (category.type != categoryType)
                {
                    Debug.LogError($"Did not find valid category to add category.type : {category.type},   categoryType : {categoryType}");
                    continue;
                }
              
                if (category.uniqueIds.Contains(uniqueId))
                    category.uniqueIds.Remove(uniqueId);
                
                return;
            }
            Debug.LogError($"Did not find valid category to add categoryType : {categoryType}");
        }
    }
}