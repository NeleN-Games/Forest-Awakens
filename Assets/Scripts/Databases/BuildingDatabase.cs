using Enums;
using Models.Data;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Database/Building Database")]
    public class BuildingDatabase : GenericDatabase<BuildingType, BuildingData>
    {
        
    }
}
