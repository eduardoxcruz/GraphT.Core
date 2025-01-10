using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services.Specifications;

public sealed class FindReadyToStartTasksSpecification : BaseSpecification<TaskAggregate>
{
	public FindReadyToStartTasksSpecification(PagingParams pagingParams) : base(task => (task.Status == Status.ReadyToStart))
	{
		ApplyOrderByDescending(task => task.Priority);
		AddThenBy(task => task.DateTimeInfo.LimitDateTime ?? DateTimeOffset.MaxValue);
		ApplyPaging(pagingParams.PageNumber, pagingParams.PageSize);
	}
}
