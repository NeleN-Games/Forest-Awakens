using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Models.Data
{
    public abstract class CraftableAssetData<TEnum> : CommonAssetData<TEnum>
        where TEnum : System.Enum
    {
        public List<SourceRequirement> resourceRequirements;
        protected override bool IsValid()
        {
            return base.IsValid() && resourceRequirements != null && resourceRequirements.Count > 0;
        }
    }
}
