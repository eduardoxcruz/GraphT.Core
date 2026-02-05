using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Events;

public class StatusChangelogCreatedDomainEvent : DomainEvent
{
	public StatusChangelog Changelog { get; }
	
	public StatusChangelogCreatedDomainEvent(StatusChangelog changelog)
	{
		Changelog = changelog;
	}
}
