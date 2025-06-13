using System;
using System.Collections.Generic;
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
       public override CommonAssetData<ItemType> Clone()
       {
           var clone = Instantiate(this);
           return clone;
       }

       public override void Craft()
       {
           throw new NotImplementedException();
       }
    }
}