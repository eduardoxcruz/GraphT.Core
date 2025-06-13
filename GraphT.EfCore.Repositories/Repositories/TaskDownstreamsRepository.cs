using GraphT.EfCore.Repositories.Models;
using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Repositories;

public class TaskDownstreamsRepository : ITaskDownstreamsRepository
{
	private readonly EfDbContext _context;

	public TaskDownstreamsRepository(EfDbContext context)
	{
		_context = context;
	}

	public async ValueTask<PagedList<TodoTask>> FindTaskDownstreamsById(Guid id)
	{
		List<TodoTask> results = await _context.TaskStreams
			.Where(td => td.UpstreamId == id)
			.Select(td => td.Downstream)
			.AsNoTracking()
			.ToListAsync();

		await StreamsPopulator.PopulateStreamCountsAsync(results, _context);
		
		return new PagedList<TodoTask>(results, results.Count, 1, results.Count);
	}

	public async ValueTask AddDownstreamAsync(Guid taskId, Guid downstreamId)
	{
		TaskStream stream = new(taskId, downstreamId);

		if (!await _context.TaskStreams.AnyAsync(ts => ts.DownstreamId == downstreamId && ts.UpstreamId == taskId))
		{
			await _context.TaskStreams.AddAsync(stream);
		}
	}

	public async ValueTask RemoveDownstreamAsync(Guid taskId, Guid downstreamId)
	{
		TaskStream stream = new(taskId, downstreamId);
		
		if (await _context.TaskStreams.AnyAsync(ts => ts.DownstreamId == downstreamId && ts.UpstreamId == taskId))
		{
			_context.TaskStreams.Remove(stream);
		}
	}
}
