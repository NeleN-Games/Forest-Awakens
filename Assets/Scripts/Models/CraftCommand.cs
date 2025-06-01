namespace Models
{
    public class CraftCommand<TID>
    {
        public TID ID { get; }

        public CraftCommand(TID id)
        {
            ID = id;
        }
    }
}