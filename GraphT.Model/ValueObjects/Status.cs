namespace GraphT.Model.ValueObjects;

public enum Status
{
	Created = 0,
	Backlog = 1,
	ReadyToStart = 2,
	InProgress = 3,
	Paused = 4,
	Dropped = 5,
	Completed = 6
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
			Status.Completed => "\u2705 Completed",
			_ => "\u270c Created"
		};

		return formatedName;
	}
}
