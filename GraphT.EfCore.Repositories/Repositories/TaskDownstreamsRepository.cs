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

		return new PagedList<TodoTask>(results, results.Count, 1, results.Count);
	}
}
