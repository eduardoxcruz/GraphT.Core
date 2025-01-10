namespace GraphT.Model.Aggregates;

public class LifeArea
{
	public Guid Id { get; private set; }
	public string Name { get; set; }
	private HashSet<TodoTask> _tasks;
	public IReadOnlySet<TodoTask> Tasks => _tasks;

	public LifeArea(string name)
	{
		Id = Guid.NewGuid();
		Name = name;
		_tasks = new HashSet<TodoTask>();
	}
}
