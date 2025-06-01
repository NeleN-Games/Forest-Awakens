using Databases;
using Enums;
using Models.Data;
using UnityEngine;

namespace Managers
{
    public class ItemCrafter : Crafter<ItemType, ItemData, ItemDatabase>
    {
        protected override void HandleCraftSuccess(ItemData data)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnCraftSuccess(ItemData data)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnCraftFailure(ItemData data)
        {
            throw new System.NotImplementedException();
        }
    }
}
