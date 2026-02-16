using SeedWork;

namespace GraphT.Model.Events;

public class ChildedRemovedDomainEvent : DomainEvent
{
	public Guid ChildId { get; }
	
	public ChildedRemovedDomainEvent(Guid childId)
	{
		ChildId = childId;
	}
}
