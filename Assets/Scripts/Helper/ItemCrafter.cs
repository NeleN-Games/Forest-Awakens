using Databases;
using Enums;
using Managers;
using Models.Data;
using Services;

namespace Helper
{
    public class ItemCrafter : Crafter<ItemType, ItemData, ItemDatabase>
    {
        public override void Initialize()
        {
            Database = ServiceLocator.Get<ItemDatabase>();
            OnCraft += Craft;
        }

        public override void OnDestroy()
        {
            OnCraft -= Craft;
        }

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
