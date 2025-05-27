using SeedWork;

namespace GraphT.Model.Entities;

public class LifeArea : Entity
{
	private LifeArea() : this("New Life Area") { }
	
	public LifeArea(string name, Guid? id = null) : base(name, id) { }
}
