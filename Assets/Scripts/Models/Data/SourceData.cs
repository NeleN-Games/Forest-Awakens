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
        public override SourceType GetID() => enumType;
        protected override bool IsValid()
        {
            return prefab != null 
                   && icon != null;
        }
    }
}
