namespace SeedWork;

public interface IUnitOfWork
{
	IRepository<TEntity> Repository<TEntity>() where TEntity : class;
	
	ValueTask<int> SaveChangesAsync();
}
