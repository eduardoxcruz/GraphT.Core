using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GraphT.EfCore.Repositories;

public class TaskLogRepository : ITaskLogRepository
{
	private readonly EfDbContext _context;

	public TaskLogRepository(EfDbContext context)
	{
		_context = context;
	}

	public async ValueTask<TaskLog?> FindTaskLastLog(Guid taskId)
	{
	    return await _context.TaskLogs
	        .Where(taskLog => taskLog.TaskId.Equals(taskId))
	        .OrderByDescending(taskLog => taskLog.DateTime)
	        .FirstOrDefaultAsync(); 
	}

	public async ValueTask AddAsync(TaskLog taskLog)
	{
		await _context.TaskLogs.AddAsync(taskLog);
	}

	public async ValueTask AddRangeAsync(IEnumerable<TaskLog> taskLogs)
	{
		await _context.TaskLogs.AddRangeAsync(taskLogs);
	}

	public async ValueTask RemoveAsync(TaskLog taskLog)
	{
		_context.TaskLogs.Remove(taskLog);
		await Task.CompletedTask;
	}

	public async ValueTask RemoveRangeAsync(IEnumerable<TaskLog> taskLogs)
	{
		_context.TaskLogs.RemoveRange(taskLogs);
		await Task.CompletedTask;
	}
}
