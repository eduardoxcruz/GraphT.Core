namespace GraphT.Model.ValueObjects;

public class OldTaskLog
{
	public readonly Guid TaskId;
	public readonly DateTimeOffset DateTime;
	public readonly OldStatus OldStatus;
	public readonly TimeSpan? TimeSpentOnTask;

	private OldTaskLog() { }

	public OldTaskLog(Guid taskId, DateTimeOffset dateTime, OldStatus oldStatus, TimeSpan? timeSpend = null)
	{
		TaskId = taskId;
		DateTime = dateTime;
		OldStatus = oldStatus;
		TimeSpentOnTask = timeSpend;
	}
}
