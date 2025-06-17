namespace SeedWork;

public class Entity
{
	public Guid Id { get; private set; }
	public string Name { get; set; }

	private Entity()
	{
		Id = Guid.NewGuid();
		Name = "New Entity";
	}

	protected Entity(string name, Guid? id = null)
	{
		Id = id ?? Guid.NewGuid();
		Name = name;
	}
}
