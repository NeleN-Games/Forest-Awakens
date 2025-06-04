using System.Collections.Generic;
using Models;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(menuName = "Database/Unique Id Database")]
    public class UniqueIdDatabase : ScriptableObject
    {
       public List<UniqueId> uniqueIds;
    }
}
