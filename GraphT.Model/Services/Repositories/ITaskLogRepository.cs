using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services.Repositories;

public interface ITaskLogRepository
{
	ValueTask<TaskLog?> FindTaskLastLog(Guid taskId);
	ValueTask AddAsync(TaskLog task);
	ValueTask AddRangeAsync(IEnumerable<TaskLog> taskLogs);
	ValueTask RemoveAsync(TaskLog task);
	ValueTask RemoveRangeAsync(IEnumerable<TaskLog> taskLogs);
	
}
