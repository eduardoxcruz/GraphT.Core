using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services.Specifications;

public sealed class TasksWhereStatusIsReadyToStartSpecification : BaseSpecification<TodoTask>
{
	public TasksWhereStatusIsReadyToStartSpecification(PagingParams pagingParams) : base(task => (task.Status == Status.ReadyToStart))
	{
		ApplyOrderByDescending(task => task.Priority);
		AddThenBy(task => task.DateTimeInfo.LimitDateTime ?? DateTimeOffset.MaxValue);
		AddInclude(task => (task.Downstreams.Count == 0) || (task.Progress >= 99));
		ApplyPaging(pagingParams.PageNumber, pagingParams.PageSize);
	}
}
