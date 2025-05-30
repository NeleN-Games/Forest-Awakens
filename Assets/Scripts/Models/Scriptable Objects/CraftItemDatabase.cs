using System.Collections.Generic;
using Models.Data;
using UnityEngine;

namespace Models.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "CraftItemDatabase", menuName = "Crafting/Item Database")]
    public class CraftItemDatabase : ScriptableObject
    {
        public List<CraftItemData> items = new List<CraftItemData>();
    }
}