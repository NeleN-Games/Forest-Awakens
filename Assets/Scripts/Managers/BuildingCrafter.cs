using Databases;
using Enums;
using Models.Data;
using UnityEngine;

namespace Managers
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
