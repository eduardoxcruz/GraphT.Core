using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.EfCore.Repositories.Repositories;

public class LifeAreasRepository : ILifeAreasRepository
{
	private readonly EfDbContext _context;

	public LifeAreasRepository(EfDbContext context)
	{
		_context = context;
	}

	public ValueTask<PagedList<LifeArea>> FindTaskLifeAreasById(Guid id)
	{
		throw new NotImplementedException();
	}

	public ValueTask LinkTaskAsync(Guid lifeAreaId, Guid taskId)
	{
		throw new NotImplementedException();
	}

	public ValueTask RemoveTaskAsync(Guid lifeAreaId, Guid taskId)
	{
		throw new NotImplementedException();
	}

	public ValueTask AddAsync(LifeArea lifeArea)
	{
		throw new NotImplementedException();
	}

	public ValueTask AddRangeAsync(IEnumerable<LifeArea> lifeAreas)
	{
		throw new NotImplementedException();
	}

	public ValueTask RemoveAsync(LifeArea lifeArea)
	{
		throw new NotImplementedException();
	}

	public ValueTask RemoveRangeAsync(IEnumerable<LifeArea> lifeAreas)
	{
		throw new NotImplementedException();
	}

	public ValueTask UpdateAsync(LifeArea lifeArea)
	{
		throw new NotImplementedException();
	}

	public ValueTask UpdateRangeAsync(IEnumerable<LifeArea> lifeAreas)
	{
		throw new NotImplementedException();
	}

	public ValueTask<bool> ContainsAsync(Guid id)
	{
		throw new NotImplementedException();
	}
}
