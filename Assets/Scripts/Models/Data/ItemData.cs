using System;
using Enums;
using Interfaces;
using UnityEngine;

namespace Models.Data
{
    [CreateAssetMenu(menuName = "Data/Item")]
    [Serializable]
    public class ItemData : CraftableAssetData<ItemType>, ICraftable<ItemType>, IIdentifiable<ItemType>
    {
       public override ItemType GetID() => enumType;
    }
}