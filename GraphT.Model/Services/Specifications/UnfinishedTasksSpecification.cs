using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services.Specifications;

public sealed class UnfinishedTasksSpecification : BaseSpecification<TaskAggregate>
{
	public UnfinishedTasksSpecification(string? name, PagingParams pagingParams) : 
		base(task => (task.Status != Status.Completed) && (name == null || task.Name.Contains(name)))
	{
		ApplyPaging(pagingParams.PageNumber, pagingParams.PageSize);
	}
}
