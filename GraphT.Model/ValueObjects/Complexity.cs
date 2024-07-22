namespace GraphT.Model.ValueObjects;

public enum Complexity
{
	Indefinite = 0,
	Low = 1,
	High = 2
}

public static class ComplexityExtensions
{
	public static string FormatedName(this Complexity complexity)
	{
		string formatedName = complexity switch
		{
			Complexity.Indefinite => "\ud83e\udd37 Indefinite",
			Complexity.Low => "\ud83e\udd71 Low",
			_ => "\ud83d\ude31 High"
		};
		
		return formatedName;
	}
}
