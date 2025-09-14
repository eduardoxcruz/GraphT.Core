namespace GraphT.Model.ValueObjects;

public class TaskLog
{
	public readonly Guid TaskId;
	public readonly DateTimeOffset DateTime;
	public readonly OldStatus OldStatus;
	public readonly TimeSpan? TimeSpentOnTask;

	private TaskLog() { }

	public TaskLog(Guid taskId, DateTimeOffset dateTime, OldStatus oldStatus, TimeSpan? timeSpend = null)
	{
		TaskId = taskId;
		DateTime = dateTime;
		OldStatus = oldStatus;
		TimeSpentOnTask = timeSpend;
	}
}
