using GraphT.Model.Aggregates;
using GraphT.Model.Entities;

namespace GraphT.Model.Repositories;

public interface ITaskReadableRepository
{
	public TaskAggregate GetTask(Guid taskId);
	public HashSet<TodoTask> GetUpstreams(Guid taskId);
	public HashSet<TodoTask> GetDownstreams(Guid taskId);
}
