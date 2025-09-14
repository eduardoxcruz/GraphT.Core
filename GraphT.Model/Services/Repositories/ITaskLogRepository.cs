using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services.Repositories;

public interface ITaskLogRepository
{
	ValueTask<OldTaskLog?> FindTaskLastLog(Guid taskId);
	ValueTask AddAsync(OldTaskLog oldTask);
	ValueTask AddRangeAsync(IEnumerable<OldTaskLog> taskLogs);
	ValueTask RemoveAsync(OldTaskLog oldTask);
	ValueTask RemoveRangeAsync(IEnumerable<OldTaskLog> taskLogs);
	
}
