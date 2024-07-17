namespace GraphT.Model.Entities;

public class TodoTask
{
	public string Id { get; private set; }
	public string Name { get; set; }

	protected TodoTask(string name)
	{
		Id = Guid.NewGuid().ToString();
		Name = name;
	}
}
