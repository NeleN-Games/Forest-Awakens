using System;
using System.Collections.Generic;
using Interfaces;
using Models;
using Models.Data;
using UnityEngine;

namespace Databases
{
    public abstract class GenericDatabase<TEnum, TData> : ScriptableObject,IInitializable
        where TEnum : Enum
        where TData : CommonAssetData<TEnum>
    {
        [SerializeField] protected List<TData> entries = new();
        public List<TData> Entries => entries;
        protected  Dictionary<TEnum, TData> DataDict;
        protected Dictionary<UniqueId, TData> UniqueIdLookup;

        public virtual void Initialize()
        {
            DataDict = new Dictionary<TEnum, TData>();
            UniqueIdLookup = new Dictionary<UniqueId, TData>();

            foreach (var entry in entries)
            {
                
                if (entry == null) continue;

                var id = entry.GetEnum();
                if (!DataDict.ContainsKey(id))
                {
                    DataDict[id] = entry;
                }
                else
                {
                    Debug.LogWarning($"Duplicate ID {id} in {typeof(TData)}");
                }

                // Register UniqueId
                if (entry is CraftableAssetData<TEnum> craftable && craftable.GetUniqueId() != null)
                {
                    var uid = craftable.GetUniqueId();
                    if (!UniqueIdLookup.ContainsKey(uid))
                    {
                        UniqueIdLookup[uid] = entry;
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate UniqueId {uid.ID} in {typeof(TData)}");
                    }
                }
            }
        }

        public void OnDestroy(){}
        public TData Get(TEnum id)
        {
            if (DataDict != null && DataDict.TryGetValue(id, out var data))
                return data;

            Debug.LogError($"No data found for ID {id} in {typeof(TData)}");
            return null;
        }

        public TData GetByUniqueId(UniqueId uniqueId)
        {
            if (UniqueIdLookup != null && uniqueId != null && UniqueIdLookup.TryGetValue(uniqueId, out var data))
                return data;

            Debug.LogError($"No data found with UniqueId {uniqueId?.ID} in {typeof(TData)}");
            return null;
        }

        public bool Has(TEnum id)
        {
            return DataDict != null && DataDict.ContainsKey(id);
        }
        
        public virtual bool Remove(TData item)
        {
            if (item == null) return false;

            var id = item.GetEnum();

            if (DataDict != null && DataDict.ContainsKey(id))
            {
                DataDict.Remove(id);
            }

            return entries.Remove(item);
        }

        public virtual bool RemoveByID(TEnum id)
        {
            if (DataDict == null || !DataDict.ContainsKey(id))
                return false;

            var item = DataDict[id];
            DataDict.Remove(id);
            return entries.Remove(item);
        }
      
    }
}