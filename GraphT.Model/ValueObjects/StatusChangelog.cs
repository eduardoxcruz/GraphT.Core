namespace GraphT.Model.ValueObjects;

public struct StatusChangelog
{
	public DateTimeOffset CreationDateTime { get; }
	public DateTimeOffset ChangeDateTime { get; }
	public Status Status { get; }

	public StatusChangelog(DateTimeOffset changeDateTime, Status status)
	{
		CreationDateTime = DateTimeOffset.Now;
		ChangeDateTime = changeDateTime;
		Status = status;
	}
}
