using SeedWork;

namespace Core.Model.Events;

public class ParentAddedDomainEvent : DomainEvent
{
	public Guid ParentId { get; }

	public ParentAddedDomainEvent(Guid parentId)
	{
		ParentId = parentId;
	}
}
