using System.Linq.Expressions;

namespace SeedWork;

public abstract class BaseSpecification<T> : ISpecification<T>
{
	public int PageNumber { get; private set; }
	public int PageSize { get; private set; }
	public bool IsPagingEnabled { get; private set; } = false;
	public List<string> IncludeStrings { get; }
	public List<Expression<Func<T, object>>> Includes { get; }
	public Expression<Func<T, bool>>? Criteria { get; }
	public Expression<Func<T, object>>? OrderBy { get; private set; }
	public Expression<Func<T, object>>? OrderByDescending { get; private set; }
	public Expression<Func<T, object>>? GroupBy { get; private set; }

	protected BaseSpecification()
	{
		IncludeStrings = new List<string>();
		Includes = new List<Expression<Func<T, object>>>();
	}

	protected BaseSpecification(Expression<Func<T, bool>> criteria)
	{
		IncludeStrings = new List<string>();
		Includes = new List<Expression<Func<T, object>>>();
		Criteria = criteria;
	}

	protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
	{
		Includes.Add(includeExpression);
	}

	protected virtual void AddInclude(string includeString)
	{
		IncludeStrings.Add(includeString);
	}

	protected virtual void ApplyPaging(int pageNumber, int pageSize)
	{
		if (pageNumber <= 0 || pageSize <= 0)
		{
			return;
		}

		PageNumber = pageNumber;
		PageSize = pageSize;
		IsPagingEnabled = true;
	}

	protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
	{
		OrderBy = orderByExpression;
	}

	protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
	{
		OrderByDescending = orderByDescendingExpression;
	}

	protected virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
	{
		GroupBy = groupByExpression;
	}
}
