namespace GraphT.Model.ValueObjects;

public enum Status
{
	Backlog = 0,
	ReadyToStart = 1,
	InProgress = 2,
	Paused = 3,
	Dropped = 4,
	Completed = 5
}

public static class StatusExtensions
{
	public static string FormatedName(this Status status)
	{
		string formatedName = status switch
		{
			Status.Backlog => "\ud83d\udcdd Backlog",
			Status.ReadyToStart => "\ud83d\udc4c Ready To Start",
			Status.InProgress => "\u25b6 In Progress",
			Status.Paused => "\u23f8 Paused",
			Status.Dropped => "\ud83d\uddd1 Dropped",
			_ => "\u2705 Completed"
		};

		return formatedName;
	}
}
