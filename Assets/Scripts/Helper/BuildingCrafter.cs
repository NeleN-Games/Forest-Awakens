using Databases;
using Enums;
using Managers;
using Models.Data;

namespace Helper
{
    public class BuildingCrafter : Crafter<BuildingType, BuildingData, BuildingDatabase>
    {

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
