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
	
	public async ValueTask<PagedList<LifeArea>> FindTaskLifeAreasById(Guid id)
	{
		throw new NotImplementedException();
	}
}
