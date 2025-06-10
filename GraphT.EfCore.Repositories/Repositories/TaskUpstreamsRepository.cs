using GraphT.EfCore.Repositories.Models;
using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Repositories;

public class TaskUpstreamsRepository : ITaskUpstreamsRepository
{
	private readonly EfDbContext _context;

	public TaskUpstreamsRepository(EfDbContext context)
	{
		_context = context;
	}

	public async ValueTask<PagedList<TodoTask>> FindTaskUpstreamsById(Guid id)
	{
		List<TodoTask> results = await _context.TaskStreams
			.Where(td => td.DownstreamId == id)
			.Select(td => td.Upstream)
			.AsNoTracking()
			.ToListAsync();

		return new PagedList<TodoTask>(results, results.Count, 1, results.Count);
	}
	
	public async ValueTask<PagedList<TodoTask>> FindTasksWithoutUpstreams(PagingParams pagingParams)
	{
		IQueryable<TodoTask> query = _context.TodoTasks
			.Where(task => !_context.TaskStreams.Any(ts => ts.DownstreamId == task.Id))
			.OrderByDescending(task => task.DateTimeInfo.CreationDateTime)
			.AsNoTracking();

		int totalCount = await query.CountAsync();
       
		List<TodoTask> results = await query
			.Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
			.Take(pagingParams.PageSize)
			.ToListAsync();

		return new PagedList<TodoTask>(results, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
	}

	public async ValueTask AddUpstreamAsync(Guid taskId, Guid upstreamId)
	{
		TaskStream stream = new(upstreamId, taskId);
		
		if (!await _context.TaskStreams.AnyAsync(ts => ts.UpstreamId == upstreamId && ts.DownstreamId == taskId))
		{
			await _context.TaskStreams.AddAsync(stream);
		}
	}

	public async ValueTask RemoveUpstreamAsync(Guid taskId, Guid upstreamId)
	{
		TaskStream stream = new(upstreamId, taskId);
		
		if (await _context.TaskStreams.AnyAsync(ts => ts.DownstreamId == taskId && ts.UpstreamId == upstreamId))
		{
			_context.TaskStreams.Remove(stream);
		}
	}
}
