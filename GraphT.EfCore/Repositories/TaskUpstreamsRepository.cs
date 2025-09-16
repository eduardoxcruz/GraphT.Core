using GraphT.EfCore.Models;
using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories;

public class TaskUpstreamsRepository : ITaskUpstreamsRepository
{
	private readonly EfDbContext _context;

	public TaskUpstreamsRepository(EfDbContext context)
	{
		_context = context;
	}

	public async ValueTask<PagedList<OldTodoTask>> FindTaskUpstreamsById(Guid id)
	{
		List<OldTodoTask> results = await _context.TaskStreams
			.Where(td => td.DownstreamId == id)
			.Select(td => td.Upstream)
			.AsNoTracking()
			.ToListAsync();

		await StreamsPopulator.PopulateStreamCountsAsync(results, _context);
		
		return new PagedList<OldTodoTask>(results, results.Count, 1, results.Count);
	}
	
	public async ValueTask<PagedList<OldTodoTask>> FindTasksWithoutUpstreams(PagingParams pagingParams)
	{
		IQueryable<OldTodoTask> query = _context.TodoTasks
			.Where(task => !_context.TaskStreams.Any(ts => ts.DownstreamId == task.Id))
			.OrderByDescending(task => task.OldDateTimeInfo.CreationDateTime)
			.AsNoTracking();

		int totalCount = await query.CountAsync();
       
		List<OldTodoTask> results = await query
			.Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
			.Take(pagingParams.PageSize)
			.ToListAsync();

		await StreamsPopulator.PopulateStreamCountsAsync(results, _context);
		
		return new PagedList<OldTodoTask>(results, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
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
