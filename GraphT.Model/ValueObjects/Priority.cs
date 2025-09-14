namespace GraphT.Model.ValueObjects;

public struct Priority
{
	public int Index { get; }
	public string Name { get; }
	public string Emoji { get; }
	
	public static readonly Priority Distraction = new(0, "Distraction", "\ud83d\udecc");
	public static readonly Priority Consider = new(1, "Consider", "\ud83e\uddcd\u200d\u2642\ufe0f");
	public static readonly Priority Urgent = new(2, "Urgent", "\ud83c\udfc3\u200d\u2642\ufe0f");
	public static readonly Priority Critical = new(3, "Critical", "\ud83d\udeb4\u200d\u2642\ufe0f");

	public Priority()
	{
		Index = Distraction.Index;
		Name = Distraction.Name;
		Emoji = Distraction.Emoji;
	}

	private Priority(int index, string name, string emoji)
	{
		Index = index;
		Name = name;
		Emoji = emoji;
	}
}
