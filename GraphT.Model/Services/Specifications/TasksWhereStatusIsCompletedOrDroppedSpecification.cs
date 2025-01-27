using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services.Specifications;

public sealed class TasksWhereStatusIsCompletedOrDroppedSpecification : BaseSpecification<TodoTask>
{
	public TasksWhereStatusIsCompletedOrDroppedSpecification(PagingParams pagingParams) : 
		base(task => task.Status == Status.Completed || task.Status == Status.Dropped)
	{
		ApplyOrderByDescending(task => task.DateTimeInfo.FinishDateTime!);
		ApplyPaging(pagingParams.PageNumber, pagingParams.PageSize);
	}
}
