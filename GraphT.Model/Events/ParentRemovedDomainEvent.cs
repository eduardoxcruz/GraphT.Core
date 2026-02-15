using SeedWork;

namespace GraphT.Model.Events;

public class ParentRemovedDomainEvent : DomainEvent
{
	public Guid ParentId { get; }

	public ParentRemovedDomainEvent(Guid parentId)
	{
		ParentId = parentId;
	}
}
