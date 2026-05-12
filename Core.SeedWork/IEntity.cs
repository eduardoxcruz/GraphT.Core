namespace Core.SeedWork;

public interface IEntity<TId> where TId : notnull
{
	public TId Id { get; }
}

