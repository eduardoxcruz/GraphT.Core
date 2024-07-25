namespace GraphT.Model.Entities;

public class LifeArea
{
	public string Name { get; set; }

	protected LifeArea(string name)
	{
		Name = name;
	}
}
