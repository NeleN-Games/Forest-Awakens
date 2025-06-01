namespace Interfaces
{
    public interface IIdentifiable<out TEnum>
    {
        TEnum GetID();
    }
}