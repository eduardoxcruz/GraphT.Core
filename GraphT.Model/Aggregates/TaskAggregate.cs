using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.Aggregates;

public class TaskAggregate(
	string name
	, Status? status = null
	, bool? isFun = null
	, bool? isProductive = null
	, Complexity? complexity = null
	, Priority? priority = null
	, Guid? id = null)
	: TodoTask(name, status, isFun, isProductive, complexity, priority, id)
{
	private HashSet<TodoTask> _upstreams = [];
	private HashSet<TodoTask> _downstreams = [];
	private HashSet<LifeArea> _lifeAreas = [];
	
	public IReadOnlySet<TodoTask> Upstreams => _upstreams;
	public IReadOnlySet<TodoTask> Downstreams => _downstreams;
	public IReadOnlySet<LifeArea> LifeAreas => _lifeAreas;

	private TaskAggregate() : this("New Task") { }
	
	public void AddUpstream(TodoTask upstream)
	{
		ValidateTask(upstream);
		
		_upstreams.Add(upstream);
	}

	public void RemoveUpstream(TodoTask upstream)
	{
		ValidateTask(upstream);
		
		_upstreams.RemoveWhere(todoTask => todoTask.Id.Equals(upstream.Id));
	}

	public void AddUpstreams(HashSet<TodoTask> upstreams)
	{
		ValidateTaskCollection(upstreams);
		
		_upstreams.UnionWith(upstreams);
	}

	public void RemoveUpstreams(HashSet<TodoTask> upstreams)
	{
		ValidateTaskCollection(upstreams);
		
		_upstreams.ExceptWith(upstreams);
	}

	public void ReplaceUpstreams(HashSet<TodoTask> newUpstreams)
	{
		ValidateTaskCollection(newUpstreams);
		
		_upstreams.Clear();
		_upstreams = newUpstreams;
	}

	public void ClearUpstreams()
	{
		if (_upstreams.Count == 1) return;
		
		_upstreams.Clear();
	}

	public void AddDownstream(TodoTask downstream)
	{
		ValidateTask(downstream);
		
		_downstreams.Add(downstream);
	}
	
	public void RemoveDownstream(TodoTask downstream)
	{
		ValidateTask(downstream);
		
		_downstreams.RemoveWhere(todoTask => todoTask.Id.Equals(downstream.Id));
	}

	public void AddDownstreams(HashSet<TodoTask> downstreams)
	{
		ValidateTaskCollection(downstreams);
		
		_downstreams.UnionWith(downstreams);
	}

	public void RemoveDownstreams(HashSet<TodoTask> downstreams)
	{
		ValidateTaskCollection(downstreams);

		_downstreams.ExceptWith(downstreams);
	}

	public void ReplaceDownstreams(HashSet<TodoTask> newDownstreams)
	{
		ValidateTaskCollection(newDownstreams);
		
		_downstreams.Clear();
		_downstreams = newDownstreams;
	}

	public void ClearDownstreams()
	{
		if (_downstreams.Count == 0) return;
		
		_downstreams.Clear();
	}
	
	private void ValidateTask(TodoTask taskAggregate)
	{
		if (taskAggregate.Id.Equals(Guid.Empty)) throw new ArgumentException("Task id cannot be empty");
	}

	private void ValidateTaskCollection(HashSet<TodoTask> taskCollection)
	{
		if (taskCollection is null) throw new ArgumentException("Task collection cannot be null");

		if (taskCollection.Count == 0) throw new ArgumentException("Task collection cannot be empty");

		if (taskCollection.Any(task => task.Id.Equals(Guid.Empty)))
			throw new ArgumentException("Task collection cannot contain tasks with empty Id");
	}
	
	public void AddLifeArea(LifeArea lifeArea)
	{
		ValidateLifeArea(lifeArea);
		
		_lifeAreas.Add(lifeArea);
	}
	
	public void RemoveLifeArea(LifeArea lifeArea)
	{
		ValidateLifeArea(lifeArea);
		
		_lifeAreas.RemoveWhere(lifeAreaAggregate => lifeAreaAggregate.Id.Equals(lifeArea.Id));
	}

	public void AddLifeAreas(HashSet<LifeArea> lifeAreas)
	{
		ValidateLifeAreaCollection(lifeAreas);
		
		_lifeAreas.UnionWith(lifeAreas);
	}

	public void RemoveLifeAreas(HashSet<LifeArea> lifeAreas)
	{
		ValidateLifeAreaCollection(lifeAreas);

		_lifeAreas.ExceptWith(lifeAreas);
	}

	public void ReplaceLifeAreas(HashSet<LifeArea> newLifeAreas)
	{
		ValidateLifeAreaCollection(newLifeAreas);
		
		_lifeAreas.Clear();
		_lifeAreas = newLifeAreas;
	}

	public void ClearLifeAreas()
	{
		if (_lifeAreas.Count == 0) return;
		
		_lifeAreas.Clear();
	}
	
	// TODO: Implement this
	private void ValidateLifeArea(LifeArea lifeArea)
	{
		if (lifeArea.Id.Equals(Guid.Empty)) throw new ArgumentException("Life Area id cannot be empty");
	}

	private void ValidateLifeAreaCollection(HashSet<LifeArea> lifeAreaCollection)
	{
		if (lifeAreaCollection is null) throw new ArgumentException("Life Area collection cannot be null");

		if (lifeAreaCollection.Count == 0) throw new ArgumentException("Life Area collection cannot be empty");

		if (lifeAreaCollection.Any(lifeAreaAggregate => lifeAreaAggregate.Id.Equals(Guid.Empty)))
			throw new ArgumentException("Life Area collection cannot contain life areas with empty Id");
	}
}
