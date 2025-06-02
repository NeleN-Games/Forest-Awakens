using System;

namespace Models
{
    [Serializable]
    public class CraftCommand<TID>
    {
        public TID ID { get; }

        public CraftCommand(TID id)
        {
            ID = id;
        }
    }
}