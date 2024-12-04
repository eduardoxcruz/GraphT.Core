namespace GraphT.Model.Aggregates;

public class LifeAreaAggregate
{
	public Guid Id { get; private set; }
	public string Name { get; set; }
	private HashSet<TaskAggregate> _tasks;
	public IReadOnlySet<TaskAggregate> Tasks => _tasks;

	private LifeAreaAggregate(string name)
	{
		Id = Guid.NewGuid();
		Name = name;
		_tasks = new HashSet<TaskAggregate>();
	}
}
