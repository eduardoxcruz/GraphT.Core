using GraphT.Model.Aggregates;
using GraphT.Model.Entities;

namespace GraphT.Model.Repositories;

public interface ITaskReadableRepository
{
	public ValueTask<TaskAggregate> GetTaskAsync(Guid taskId);
	public ValueTask<HashSet<TodoTask>> GetUpstreamsAsync(Guid taskId);
	public ValueTask<HashSet<TodoTask>> GetDownstreamsAsync(Guid taskId);
}
