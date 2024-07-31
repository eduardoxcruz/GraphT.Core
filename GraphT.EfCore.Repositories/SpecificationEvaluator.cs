using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories;

public static class SpecificationEvaluator<TEntity> where TEntity : class
{
	public static ValueTask<IQueryable<TEntity>> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity>? specification)
	{
		IQueryable<TEntity> query = inputQuery;

		if (specification is null) return ValueTask.FromResult(query);
		
		// modify the IQueryable using the specification's criteria expression
		if (specification.Criteria is not null)
		{
			query = query.Where(specification.Criteria);
		}

		// Includes all expression-based includes
		query = specification.Includes.Aggregate(query,
			(current, include) => current.Include(include));

		// Include any string-based include statements
		query = specification.IncludeStrings.Aggregate(query,
			(current, include) => current.Include(include));

		// Apply ordering if expressions are set
		if (specification.OrderBy != null)
		{
			query = query.OrderBy(specification.OrderBy);
		}
		
		if (specification.OrderByDescending != null)
		{
			query = query.OrderByDescending(specification.OrderByDescending);
		}

		if (specification.GroupBy != null)
		{
			query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
		}

		// Apply paging if enabled
		if (specification.IsPagingEnabled)
		{
			query = query
				.Skip(specification.Skip)
				.Take(specification.Take);
		}
		return ValueTask.FromResult(query);
	}
}
