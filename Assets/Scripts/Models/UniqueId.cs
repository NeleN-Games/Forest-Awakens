using System;
using Enums;
using Helper;
using Unity.Collections;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class UniqueId : IEquatable<UniqueId>
    {
        [SerializeField, ReadOnly]
        private string uniqueName;
        [SerializeField, ReadOnly]
        private int id;

        public string UniqueName
        {
            get => uniqueName;
            private set=> uniqueName = value ?? throw new ArgumentNullException(nameof(value), "UniqueName cannot be null");
        }

        public int ID
        {
            get => id;
            private set=> id = value;
        }

        public UniqueId(string uniqueName, int id)
        {
            UniqueName = uniqueName;
            ID = id;
        }

        public CraftableType GetCraftableType()
        {
            foreach (var kvp in UniqueIdRanges.Ranges)
            {
                if (ID >= kvp.Value.start && ID <= kvp.Value.end)
                    return kvp.Key;
            }
            throw new Exception($"Unknown CraftableType for ID {ID}");
        }

        public bool Equals(UniqueId other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return UniqueName == other.UniqueName && ID == other.ID;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UniqueId)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UniqueName, ID);
        }
        public static bool operator ==(UniqueId left, UniqueId right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(UniqueId left, UniqueId right)
        {
            return !(left == right);
        }
    }
}
