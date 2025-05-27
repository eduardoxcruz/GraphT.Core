using GraphT.Model.Aggregates;
using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services.Specifications;

public sealed class TasksWhereStatusIsInProgressSpecification : BaseSpecification<TodoTask>
{
	public TasksWhereStatusIsInProgressSpecification(PagingParams pagingParams) 
		: base(entity => entity.Status == Status.Doing)
	{
		ApplyOrderByDescending(task => task.Priority);
		AddThenBy(task => task.DateTimeInfo.LimitDateTime ?? DateTimeOffset.MaxValue);
		ApplyPaging(pagingParams.PageNumber, pagingParams.PageSize);
	}
}

