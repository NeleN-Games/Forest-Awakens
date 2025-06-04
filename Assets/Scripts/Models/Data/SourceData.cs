using System;
using Enums;
using Interfaces;
using UnityEngine;

namespace Models.Data
{
    [CreateAssetMenu(menuName = "Data/Source")]
    [Serializable]
    public class SourceData : CommonAssetData<SourceType>, IIdentifiable<SourceType>
    {
        public override SourceType GetEnum() => enumType;
    }
}
