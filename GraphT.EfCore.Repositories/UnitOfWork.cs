using System.Collections;

using SeedWork;

namespace GraphT.EfCore.Repositories;

public class UnitOfWork: IUnitOfWork, IDisposable
{
	private readonly EfDbContext _context;
	private Hashtable _repositories;

	public UnitOfWork(EfDbContext context)
	{
		_context = context;
		_repositories = new Hashtable();
	}

	public async ValueTask<int> SaveChanges()
	{
		return await _context.SaveChangesAsync();
	}

	public IRepository<TEntity> Repository<TEntity>() where TEntity : class
	{
		string type = typeof(TEntity).Name;

		if (_repositories.ContainsKey(type)) return ((IRepository<TEntity>)_repositories[type]!)!;
            
		Type repositoryType = typeof(Repository<>);

		object repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context)!;

		_repositories.Add(type, repositoryInstance);

		return ((IRepository<TEntity>)_repositories[type]!)!;
	}

	public void Dispose()
	{
		_context.Dispose();
	}
}
