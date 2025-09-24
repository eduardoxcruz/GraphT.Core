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

	public async ValueTask<OldTaskLog?> FindTaskLastLog(Guid taskId)
	{
	    return await _context.TaskLogs
	        .Where(taskLog => taskLog.TaskId.Equals(taskId))
	        .OrderByDescending(taskLog => taskLog.DateTime)
	        .FirstOrDefaultAsync(); 
	}

	public async ValueTask AddAsync(OldTaskLog oldTaskLog)
	{
		await _context.TaskLogs.AddAsync(oldTaskLog);
	}

	public async ValueTask AddRangeAsync(IEnumerable<OldTaskLog> taskLogs)
	{
		await _context.TaskLogs.AddRangeAsync(taskLogs);
	}

	public async ValueTask RemoveAsync(OldTaskLog oldTaskLog)
	{
		_context.TaskLogs.Remove(oldTaskLog);
		await Task.CompletedTask;
	}

	public async ValueTask RemoveRangeAsync(IEnumerable<OldTaskLog> taskLogs)
	{
		_context.TaskLogs.RemoveRange(taskLogs);
		await Task.CompletedTask;
	}
}
