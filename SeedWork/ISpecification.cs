using System.Linq.Expressions;

namespace SeedWork;

public interface ISpecification<T>
{
	int Take { get; }
	int Skip { get; }
	bool IsPagingEnabled { get; }
	List<string> IncludeStrings { get; }
	List<Expression<Func<T, object>>> Includes { get; }
	Expression<Func<T, bool>>? Criteria { get; }
	Expression<Func<T, object>>? GroupBy { get; }
	Expression<Func<T, object>>? OrderBy { get; }
	Expression<Func<T, object>>? OrderByDescending { get; }
}
