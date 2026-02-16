namespace SeedWork;

public struct Optional<T>
{
	public bool HasValue { get; }
	public T Value { get; }
    
	public Optional(T value)
	{
		Value = value;
		HasValue = true;
	}
}
