namespace GraphT.Model.ValueObjects;

public struct Complexity
{
	public int Index { get; }
	public string Name { get; }
	public string Emoji { get; }
	
	public static Complexity Indefinite = new(0, "Indefinite", "\ud83e\udd37");
	public static Complexity Low = new Complexity(1, "Low", "\ud83e\udd71");
	public static Complexity High = new Complexity(2, "High", "\ud83d\ude31");

	public Complexity()
	{
		Index = Indefinite.Index;
		Name = Indefinite.Name;
		Emoji = Indefinite.Emoji;
	}
	
	private Complexity(int index, string name, string emoji)
	{
		Index = index;
		Name = name;
		Emoji = emoji;
	}
}
