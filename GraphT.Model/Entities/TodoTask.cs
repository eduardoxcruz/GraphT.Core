namespace GraphT.Model.Entities;

public class TodoTask
{
	public Guid Id { get; private set; }
	public string Name { get; set; }

	protected TodoTask(string name)
	{
		Id = Guid.NewGuid();
		Name = name;
	}
}
