using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.Model.Services.Specifications;

public sealed class TasksWithoutUpstreamsSpecification : BaseSpecification<TodoTask>
{
	public TasksWithoutUpstreamsSpecification(PagingParams pagingParams) : base(entity => !entity.Upstreams.Any())
	{
		ApplyOrderBy(task => task.Name);
		ApplyPaging(pagingParams.PageNumber, pagingParams.PageSize);
	}
}

