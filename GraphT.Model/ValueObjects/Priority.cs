namespace GraphT.Model.ValueObjects;

public enum Priority
{
	MentalClutter = 0,
	ThinkAboutIt = 1,
	DoItNow = 2,
	DropEverythingElse = 3 
}

public static class PriorityExtensions
{
	public static string FormatedName(this Priority priority)
	{
		string formatedName = priority switch
		{
			Priority.MentalClutter => "\ud83d\udecc Mental Clutter",
			Priority.ThinkAboutIt => "\ud83e\uddcd\u200d\u2642\ufe0f Think About It",
			Priority.DoItNow => "\ud83c\udfc3\u200d\u2642\ufe0f Do It Now",
			_ => "\ud83d\udeb4\u200d\u2642\ufe0f Drop Everything Else"
		};

		return formatedName;
	} 
}
