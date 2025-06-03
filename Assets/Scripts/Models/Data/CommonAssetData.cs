using UnityEngine;

namespace Models.Data
{
    public abstract class CommonAssetData<TEnum> : ScriptableObject
        where TEnum : System.Enum
    {
        public GameObject prefab;
        public Sprite icon;
        public TEnum enumType;
        public abstract TEnum GetID();

        protected virtual bool IsValid()
        {
            return prefab != null && icon != null;
        }
    }
}