namespace SeedWork;

public interface IDomainEvent
{
	DateTime OccurredOn { get; }
	Guid EventId { get; }
}

public abstract class DomainEvent
{
	public DateTime OccurredOn { get; }
	public Guid EventId { get; }

	protected DomainEvent()
	{
		OccurredOn = DateTime.UtcNow;
		EventId = Guid.NewGuid();
	}
}

