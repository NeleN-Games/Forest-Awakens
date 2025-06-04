using System;
using System.Collections.Generic;
using Models.Data;
using UnityEngine;

namespace Models.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "CategoryDatabase", menuName = "Tools/Category Database", order = 1)]
    public class CategoryDatabase : ScriptableObject
    {
        public List<CategoryData> categories = new List<CategoryData>();
    }
}