using SeedWork;

namespace GraphT.Model.Events;

public class LifeAreaAddedDomainEvent : DomainEvent
{
	public Guid LifeAreaId { get; }

	public LifeAreaAddedDomainEvent(Guid lifeAreaId)
	{
		LifeAreaId = lifeAreaId;
	}
}
