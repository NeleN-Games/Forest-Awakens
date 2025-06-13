using Databases;
using Enums;
using Managers;
using Models.Data;
using Services;

namespace Helper
{
    public class BuildingCrafter : Crafter<BuildingType, BuildingData, BuildingDatabase>
    {
        public override void Initialize()
        {
            Database = ServiceLocator.Get<BuildingDatabase>();
            OnCraft += Craft;
        }

        public override void OnDestroy()
        {
            OnCraft -= Craft;
        }

        protected override void HandleCraftSuccess(BuildingData data)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnCraftSuccess(BuildingData data)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnCraftFailure(BuildingData data)
        {
            throw new System.NotImplementedException();
        }
    }

}
