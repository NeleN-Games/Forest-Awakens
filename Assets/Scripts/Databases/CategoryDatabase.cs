using System;
using System.Collections.Generic;
using Models.Data;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(fileName = "CategoryDatabase", menuName = "Tools/Category Database", order = 1)]
    public class CategoryDatabase : ScriptableObject
    {
        public List<CategoryData> categories = new List<CategoryData>();

        public void AddCraftableObjectToCategory(string categoryName,CraftableAssetData<Enum> craftableAsset)
        {
            foreach (var category in categories)
            {
                if (category.name != categoryName)
                    continue;
                craftableAsset.categoryType = category.type;
            } 
        }
    }
}