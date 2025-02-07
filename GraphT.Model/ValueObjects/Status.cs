namespace GraphT.Model.ValueObjects;

public enum Status
{
	Created = 0,
	Backlog = 1,
	Ready = 2,
	Doing = 3,
	Paused = 4,
	Dropped = 5,
	Completed = 6
}

public static class StatusExtensions
{
	public static string GetLabel(this Status status)
	{
		string formatedName = status switch
		{
			Status.Backlog => "\ud83d\udcdd Backlog",
			Status.Ready => "\ud83d\udc4c Ready To Start",
			Status.Doing => "\u25b6 Currently Doing",
			Status.Paused => "\u23f8 Paused",
			Status.Dropped => "\ud83d\uddd1 Dropped",
			Status.Completed => "\u2705 Completed",
			_ => "\u270c Created"
		};

		return formatedName;
	}
}
