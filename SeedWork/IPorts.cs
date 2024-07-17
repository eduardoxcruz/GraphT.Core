namespace SeedWork
{
    public interface IPort
    {
        ValueTask Handle();
    }

    public interface IPort<T>
    {
        ValueTask Handle(T dto);
    }
}
