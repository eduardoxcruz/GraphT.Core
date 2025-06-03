namespace SeedWork;

public interface IUnitOfWork
{
	ValueTask<int> SaveChangesAsync();
}
