namespace GraphT.Model.ValueObjects;

public struct Relevance
{
	public int Index { get; }
	public string Name { get; }
	public string Emoji { get; }

	public static Relevance Superfluous = new(0, "Superfluous", "\ud83d\ude12");
	public static Relevance Entertaining = new(1, "Entertaining", "\ud83e\udd24");
	public static Relevance Necessary = new(2, "Necessary", "\ud83e\uddd0");
	public static Relevance Purposeful = new(3, "Purposeful", "\ud83d\ude0e");
	
	private Relevance(int index, string name, string emoji)
	{
		Index = index;
		Name = name;
		Emoji = emoji;
	}
	
	public Relevance(bool isFun, bool isProductive)
	{
		switch (isFun, isProductive)
		{
			case (false, false):
				Index = 0;
				Name = "Superfluous";
				Emoji = "\ud83d\ude12";
				break;
			case (true, false):
				Index = 1;
				Name = "Entertaining";
				Emoji = "\ud83e\udd24";
				break;
			case (false, true):
				Index = 2;
				Name = "Necessary";
				Emoji = "\ud83e\uddd0";
				break;
			case (true, true):
				Index = 3;
				Name = "Purposeful";
				Emoji = "\ud83d\ude0e";
				break;
		}
	}
}
