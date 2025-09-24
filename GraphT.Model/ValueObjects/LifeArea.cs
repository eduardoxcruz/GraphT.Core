namespace GraphT.Model.ValueObjects;

public struct LifeArea
{
	public string Name { get; }

	public LifeArea()
	{
		throw new ArgumentNullException(nameof(Name),
			"Can not create Life Area without name. Use constructor with param instead.");
	}

	public LifeArea(string name)
	{
		Name = name;
	}
}
