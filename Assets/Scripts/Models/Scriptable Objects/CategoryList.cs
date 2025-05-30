using System.Collections.Generic;
using Models.Data;
using UnityEngine;

namespace Models.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "CategoryList", menuName = "Tools/Category List", order = 1)]
    public class CategoryList : ScriptableObject
    {
        public List<CategoryData> categories = new List<CategoryData>();
    }
}