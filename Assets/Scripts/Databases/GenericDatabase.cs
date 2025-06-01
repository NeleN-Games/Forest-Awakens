using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Databases
{
    public abstract class GenericDatabase<TEnum, TData> : ScriptableObject
        where TEnum : Enum
        where TData : class, IIdentifiable<TEnum>
    {
        protected List<TData> entries = new();
        public List<TData> Entries => entries;
        protected static Dictionary<TEnum, TData> dataDict;

        public virtual void Initialize()
        {
            dataDict = new Dictionary<TEnum, TData>();
            foreach (var entry in entries)
            {
                if (entry == null) continue;

                var id = entry.GetID();
                if (dataDict.ContainsKey(id))
                {
                    Debug.LogWarning($"Duplicate ID {id} in {typeof(TData)}");
                    continue;
                }

                dataDict[id] = entry;
            }
        }

        public static TData Get(TEnum id)
        {
            if (dataDict != null && dataDict.TryGetValue(id, out var data))
                return data;

            Debug.LogError($"No data found for ID {id} in {typeof(TData)}");
            return null;
        }

        public static bool Has(TEnum id)
        {
            return dataDict != null && dataDict.ContainsKey(id);
        }
    }
}