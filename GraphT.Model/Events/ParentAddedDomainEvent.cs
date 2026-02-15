using SeedWork;

namespace GraphT.Model.Events;

public class ParentAddedDomainEvent : DomainEvent
{
	Guid ParentId { get; }

	public ParentAddedDomainEvent(Guid parentId)
	{
		ParentId = parentId;
	}
}
