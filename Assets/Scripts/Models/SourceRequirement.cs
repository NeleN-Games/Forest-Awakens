using System;
using Enums;

namespace Models
{
    [Serializable]
    public class SourceRequirement
    {
        public SourceType sourceType;
        public int amount;
    }
}