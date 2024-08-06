using GraphT.Model.Entities;

namespace GraphT.Model.Aggregates;

public class LifeAreaAggregate : LifeArea
{
	private HashSet<TaskAggregate> _tasks;
	public IReadOnlySet<TaskAggregate> Tasks => _tasks;

	private LifeAreaAggregate(string name) : base(name)
	{
		_tasks = new HashSet<TaskAggregate>();
	}
}
