using Core.Model.ValueObjects;
using SeedWork;

namespace Core.Model.Events;

public class StatusChangelogCreatedDomainEvent : DomainEvent
{
	public StatusChangelog Changelog { get; }
	
	public StatusChangelogCreatedDomainEvent(StatusChangelog changelog)
	{
		Changelog = changelog;
	}
}
