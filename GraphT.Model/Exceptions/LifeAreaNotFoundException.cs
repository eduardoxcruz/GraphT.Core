namespace GraphT.Model.Exceptions;

public class LifeAreaNotFoundException : DomainException
{
	public Guid LifeAreaId { get; }

	public LifeAreaNotFoundException(Guid lifeAreaId) : base($"Life area with ID '{lifeAreaId}' was not found.")
	{
		LifeAreaId = lifeAreaId;
	}
	
	public LifeAreaNotFoundException(string message, Guid id) : base(message)
	{
		LifeAreaId = id;
	}

	public LifeAreaNotFoundException(string message, Exception inner, Guid id) : base(message, inner)
	{
		LifeAreaId = id;
	}
}
