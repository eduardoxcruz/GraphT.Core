using System.Linq.Expressions;

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
		query = specification
			.Includes
			.Aggregate(query, (current, include) => current.Include(include));

		// Include any string-based include statements
		query = specification
			.IncludeStrings
			.Aggregate(query, (current, include) => current.Include(include));

		// Apply ordering if expressions are set
		if (specification.OrderBy != null)
		{
			query = query.OrderBy(specification.OrderBy);
		}
		
		if (specification.OrderByDescending != null)
		{
			query = query.OrderByDescending(specification.OrderByDescending);
		}

		if (query is IOrderedQueryable<TEntity> orderedQuery)
        {
            for (int i = 0; i < specification.ThenByExpressions.Count; i++)
            {
                Expression<Func<TEntity, object>> expression = specification.ThenByExpressions[i];
                bool isDescending = specification.ThenByDescendingFlags[i];

                orderedQuery = isDescending ? orderedQuery.ThenByDescending(expression) : orderedQuery.ThenBy(expression);
            }
            query = orderedQuery;
        }

		if (specification.GroupBy != null)
		{
			query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
		}

		return ValueTask.FromResult(query);
	}
}
