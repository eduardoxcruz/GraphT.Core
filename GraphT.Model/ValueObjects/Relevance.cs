namespace GraphT.Model.ValueObjects;

public enum Relevance { 
	Superfluous = 0,
	Entertaining = 1,
	Necessary = 2,
	Purposeful = 3 
}

public static class RelevanceExtensions
{
	public static string FormatedName(this Relevance relevance)
	{
		string formatedName = relevance switch
		{
			Relevance.Superfluous => "\ud83d\ude12 Superfluous",
			Relevance.Entertaining => "\ud83e\udd24 Entertaining",
			Relevance.Necessary => "\ud83e\uddd0 Necessary",
			_ => "\ud83d\ude0e Purposeful"
		};

		return formatedName;
	}
}
