using SeedWork;

namespace Core.Model.Events;

public class ChildAddedDomainEvent : DomainEvent
{
	public Guid ChildId { get; }

	public ChildAddedDomainEvent(Guid childId)
	{
		ChildId = childId;
	}
}
