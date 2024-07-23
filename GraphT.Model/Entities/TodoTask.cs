namespace GraphT.Model.Entities;

public class TodoTask
{
	public Guid Id { get; private set; }
	public string Name { get; set; }

	private TodoTask()
	{
		Id = Guid.Empty;
		Name = string.Empty;
	}
	
	public TodoTask(string name)
	{
		Id = Guid.NewGuid();
		Name = name;
	}
}
