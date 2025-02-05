namespace GraphT.Model.ValueObjects;

public enum Priority
{
	Distraction = 0,
	Consider = 1,
	Urgent = 2,
	Critical = 3 
}

public static class PriorityExtensions
{
	public static string FormatedName(this Priority priority)
	{
		string formatedName = priority switch
		{
			Priority.Distraction => "\ud83d\udecc Distraction",
			Priority.Consider => "\ud83e\uddcd\u200d\u2642\ufe0f Consider",
			Priority.Urgent => "\ud83c\udfc3\u200d\u2642\ufe0f Urgent",
			_ => "\ud83d\udeb4\u200d\u2642\ufe0f Critical"
		};

		return formatedName;
	} 
}
