using SeedWork;

namespace GraphT.Model.Events;

public class ChildRemovedDomainEvent : DomainEvent
{
	public Guid ChildId { get; }

	public ChildRemovedDomainEvent(Guid childId)
	{
		ChildId = childId;
	}
}
