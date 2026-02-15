using SeedWork;

namespace GraphT.Model.Events;

public class LifeAreaRemovedDomainEvent : DomainEvent
{
	public Guid LifeAreaId { get; }

	public LifeAreaRemovedDomainEvent(Guid lifeAreaId)
	{
		LifeAreaId = lifeAreaId;
	}
}
