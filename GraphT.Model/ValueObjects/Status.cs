namespace GraphT.Model.ValueObjects;

public struct Status
{
	public int Index { get; }
	public string Name { get; }
	public string Emoji { get; }
	
	public static readonly Status Created = new(0, "Created", "\u270c");
	public static readonly Status Backlog = new(1, "Backlog", "\ud83d\udcdd");
	public static readonly Status ReadyToStart = new(2, "Ready To Start", "\ud83d\udc4c");
	public static readonly Status CurrentlyDoing = new(3, "Currently Doing", "\u25b6");
	public static readonly Status Paused = new(4, "Paused", "\u23f8");
	public static readonly Status Dropped = new(5, "Dropped", "\ud83d\uddd1");
	public static readonly Status Completed = new(6, "Completed", "\u2705");

	public Status()
	{
		Index = Created.Index;
		Name = Created.Name;
		Emoji = Created.Emoji;
	}

	private Status(int index, string name, string emoji)
	{
		Index = index;
		Name = name;
		Emoji = emoji;
	}
}
