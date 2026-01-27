using GraphT.Model.Enums;

namespace GraphT.Model.ValueObjects;

public record StatusChangelog
{
	public DateTimeOffset CreatedAt { get; }
	public DateTimeOffset ChangeEstablishedAt { get; }
	public TaskState NewState { get; }

	private StatusChangelog() { }
	
	public StatusChangelog(DateTimeOffset changeEstablishedAt, TaskState newState)
	{
		CreatedAt = DateTimeOffset.Now;
		ChangeEstablishedAt = changeEstablishedAt;
		NewState = newState;
	}
}
