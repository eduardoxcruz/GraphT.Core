namespace GraphT.Model.Aggregates;

public class LifeAreaAggregate
{
	public string Name { get; set; }
	private HashSet<TaskAggregate> _tasks;
	public IReadOnlySet<TaskAggregate> Tasks => _tasks;

	private LifeAreaAggregate(string name)
	{
		Name = name;
		_tasks = new HashSet<TaskAggregate>();
	}
}
