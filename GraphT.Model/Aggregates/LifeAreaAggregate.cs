using GraphT.Model.Entities;

namespace GraphT.Model.Aggregates;

public class LifeAreaAggregate(string name, Guid? id = null) : LifeArea(name, id)
{
	private HashSet<TodoTask> _tasks = [];
	public IReadOnlySet<TodoTask> Tasks => _tasks;
	
	private LifeAreaAggregate() : this("New Life Area") { }
}
