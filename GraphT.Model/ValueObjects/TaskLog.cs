namespace GraphT.Model.ValueObjects;

public class TaskLog
{
	public readonly Guid TaskId;
	public readonly DateTimeOffset DateTime;
	public readonly Status Status;
	public readonly TimeSpan? TimeSpentOnTask;

	private TaskLog() { }

	public TaskLog(Guid taskId, DateTimeOffset dateTime, Status status, TimeSpan? timeSpend = null)
	{
		TaskId = taskId;
		DateTime = dateTime;
		Status = status;
		TimeSpentOnTask = timeSpend;
	}
}
