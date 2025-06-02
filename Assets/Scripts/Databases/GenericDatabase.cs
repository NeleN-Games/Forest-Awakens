using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Databases
{
    public abstract class GenericDatabase<TEnum, TData> : ScriptableObject,IInitializable
        where TEnum : Enum
        where TData : class, IIdentifiable<TEnum>
    {
        [SerializeField] protected List<TData> entries = new();
        public List<TData> Entries => entries;
        protected  Dictionary<TEnum, TData> DataDict;

        public virtual void Initialize()
        {
            DataDict = new Dictionary<TEnum, TData>();
            foreach (var entry in entries)
            {
                if (entry == null) continue;

                var id = entry.GetID();
                if (DataDict.ContainsKey(id))
                {
                    Debug.LogWarning($"Duplicate ID {id} in {typeof(TData)}");
                    continue;
                }

                DataDict[id] = entry;
            }
        }

        public TData Get(TEnum id)
        {
            if (DataDict != null && DataDict.TryGetValue(id, out var data))
                return data;

            Debug.LogError($"No data found for ID {id} in {typeof(TData)}");
            return null;
        }

        public bool Has(TEnum id)
        {
            return DataDict != null && DataDict.ContainsKey(id);
        }
        
        public virtual bool Remove(TData item)
        {
            if (item == null) return false;

            var id = item.GetID();

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