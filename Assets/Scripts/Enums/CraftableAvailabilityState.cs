namespace Enums
{
    public enum CraftableAvailabilityState
    {
        Locked,         // Technology did not discover.
        Unavailable,    // Technology discovered but has not enough sources to craft. 
        Available       // Available to craft.
    }
}