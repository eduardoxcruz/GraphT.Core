using System.Linq.Expressions;

namespace SeedWork;

public interface IRepository<TEntity> where TEntity : class
{
	ValueTask<TEntity?> FindByIdAsync(params object?[]? id);
	ValueTask<IEnumerable<TEntity>> Find(ISpecification<TEntity>? specification = null);
	ValueTask AddAsync(TEntity entity);
	ValueTask AddRangeAsync(IEnumerable<TEntity> entities);
	ValueTask RemoveAsync(TEntity entity);
	ValueTask RemoveRangeAsync(IEnumerable<TEntity> entities);
	ValueTask UpdateAsync(TEntity entity);
	ValueTask<bool> ContainsAsync(ISpecification<TEntity>? specification = null);
	ValueTask<bool> ContainsAsync(Expression<Func<TEntity, bool>> predicate);
	ValueTask<int> CountAsync(ISpecification<TEntity>? specification = null);
	ValueTask<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
}
