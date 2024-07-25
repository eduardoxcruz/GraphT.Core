using GraphT.Model.ValueObjects;

namespace GraphT.Model.Entities;

public class TodoTask
{
	public Guid Id { get; private set; }
	public string Name { get; set; }
	public Status Status { get; set; }

	private TodoTask()
	{
		Id = Guid.Empty;
		Name = string.Empty;
	}
	
	public TodoTask(string name, Status status = Status.Backlog)
	{
		Id = Guid.NewGuid();
		Name = name;
		Status = status;
	}
}
