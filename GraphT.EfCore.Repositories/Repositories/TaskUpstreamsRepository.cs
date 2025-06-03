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
			.OrderBy(task => task.Name)
			.AsNoTracking();

		int totalCount = await query.CountAsync();
       
		List<TodoTask> results = await query
			.Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
			.Take(pagingParams.PageSize)
			.ToListAsync();

		return new PagedList<TodoTask>(results, totalCount, pagingParams.PageNumber, pagingParams.PageSize);
	}
}
