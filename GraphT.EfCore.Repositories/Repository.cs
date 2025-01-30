using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories;

public class Repository<TEntity>(DbContext context) : IRepository<TEntity> where TEntity : class
{
	public async ValueTask<TEntity?> FindByIdAsync(object id)
	{
		return await context.Set<TEntity>().FindAsync(id);
	}

	public async ValueTask<PagedList<TEntity>> FindAsync(ISpecification<TEntity>? specification = null)
	{
		IQueryable<TEntity> query = (await ApplySpecification(specification));
		int count = await query.CountAsync();
		
		if (specification?.IsPagingEnabled == true)
		{
			query = query
				.Skip((specification.PageNumber - 1) * specification.PageSize)
				.Take(specification.PageSize);
		}

		return new PagedList<TEntity>(await query.ToListAsync(), count, specification?.PageNumber ?? 1, specification?.PageSize ?? count);
	}

	public async ValueTask AddAsync(TEntity entity)
	{
		await context.Set<TEntity>().AddAsync(entity);
	}

	public async ValueTask AddRangeAsync(IEnumerable<TEntity> entities)
	{
		await context.Set<TEntity>().AddRangeAsync(entities);
	}

	public ValueTask RemoveAsync(TEntity entity)
	{
		context.Set<TEntity>().Remove(entity);
		return ValueTask.CompletedTask;
	}

	public ValueTask RemoveRangeAsync(IEnumerable<TEntity> entities)
	{
		context.Set<TEntity>().RemoveRange(entities);
		return ValueTask.CompletedTask;
	}

	public ValueTask UpdateAsync(TEntity entity)
	{
		context.Set<TEntity>().Attach(entity);
		context.Entry(entity).State = EntityState.Modified;
		return ValueTask.CompletedTask;
	}

	public ValueTask UpdateRangeAsync(IEnumerable<TEntity> entities)
	{
		context.Set<TEntity>().AttachRange(entities);
		
		foreach (TEntity entity in entities)
		{
			context.Entry(entity).State = EntityState.Modified;
		}
		
		return ValueTask.CompletedTask;
	}

	public async ValueTask<bool> ContainsAsync(ISpecification<TEntity>? specification = null)
	{
		return await CountAsync(specification) > 0;
	}

	public async ValueTask<bool> ContainsAsync(Expression<Func<TEntity, bool>> predicate)
	{
		return await CountAsync(predicate) > 0;
	}

	public async ValueTask<int> CountAsync(ISpecification<TEntity>? specification = null)
	{
		return await (await ApplySpecification(specification)).CountAsync();
	}

	public async ValueTask<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
	{
		return await context.Set<TEntity>().Where(predicate).CountAsync();
	}
	
	private ValueTask<IQueryable<TEntity>> ApplySpecification(ISpecification<TEntity>? spec)
	{
		return SpecificationEvaluator<TEntity>.GetQuery(context.Set<TEntity>().AsQueryable().AsNoTracking(), spec);
	}
}
