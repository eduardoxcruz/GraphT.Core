namespace GraphT.Model.Aggregates;

public class LifeArea
{
	public Guid Id { get; private set; }
	public string Name { get; set; }
	private HashSet<TaskAggregate> _tasks;
	public IReadOnlySet<TaskAggregate> Tasks => _tasks;

	public LifeArea(string name)
	{
		Id = Guid.NewGuid();
		Name = name;
		_tasks = new HashSet<TaskAggregate>();
	}
}
