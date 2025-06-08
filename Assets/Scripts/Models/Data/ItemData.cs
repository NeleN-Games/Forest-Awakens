using System;
using Enums;
using Interfaces;
using UnityEngine;

namespace Models.Data
{
    [CreateAssetMenu(menuName = "Data/Item")]
    [Serializable]
    public class ItemData : CraftableAssetData<ItemType>
    {
       public override ItemType GetEnum() => enumType;
    }
}