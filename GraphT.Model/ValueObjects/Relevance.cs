namespace GraphT.Model.ValueObjects;

public struct Relevance
{
	public string Name { get; }
	public string Emoji { get; }

	public Relevance(bool isFun, bool isProductive)
	{
		switch (isFun, isProductive)
		{
			case (false, false):
				Name = "Superfluous";
				Emoji = "\ud83d\ude12";
				break;
			case (true, false):
				Name = "Entertaining";
				Emoji = "\ud83e\udd24";
				break;
			case (false, true):
				Name = "Necessary";
				Emoji = "\ud83e\uddd0";
				break;
			case (true, true):
				Name = "Purposeful";
				Emoji = "\ud83d\ude0e";
				break;
		}
	}
}
