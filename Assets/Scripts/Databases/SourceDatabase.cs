using Enums;
using Models.Data;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Database/Source Database")]
    public class SourceDatabase : GenericDatabase<SourceType, SourceData>
    {
        
    }
}