using GraphT.Model.Entities;

namespace GraphT.Model.Aggregates;

public class LifeAreaAggregate : LifeArea
{
	private readonly HashSet<TodoTask> _tasks;
	public IReadOnlySet<TodoTask> Tasks => _tasks;

	private LifeAreaAggregate(string name) : base(name)
	{
		_tasks = new HashSet<TodoTask>();
	}
}
