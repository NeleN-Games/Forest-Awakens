using System;
using Enums;
using Interfaces;
using UnityEngine;

namespace Models.Data
{
    [CreateAssetMenu(menuName = "Data/Source")]
    [Serializable]
    public class SourceData : CommonAssetData<SourceType>
    {
        public override SourceType GetEnum() => enumType;
        public override CommonAssetData<SourceType> Clone()
        {
            return Instantiate(this);
            
        }
    }
}
