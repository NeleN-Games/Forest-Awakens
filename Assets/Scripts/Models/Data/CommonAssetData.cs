using UnityEngine;

namespace Models.Data
{
    public abstract class CommonAssetData<TEnum> : ScriptableObject
        where TEnum : System.Enum
    {
        public GameObject prefab;
        public Sprite icon;
        public TEnum enumType;
       
        public abstract TEnum GetEnum();

        public abstract CommonAssetData<TEnum> Clone();

        public virtual void Initialize(GameObject prefab, Sprite icon, TEnum enumType)
        {
            this.prefab = prefab;
            this.icon = icon;
            this.enumType = enumType;
        }
        protected virtual bool IsValid()
        {
            return prefab != null && icon != null;
        }
    }
}