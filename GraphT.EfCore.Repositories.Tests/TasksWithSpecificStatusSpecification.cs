using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests;

public sealed class TasksWithSpecificStatusSpecification : BaseSpecification<TodoTask>
{
	public TasksWithSpecificStatusSpecification(Status taskStatus, int pageNumber = 0, int pageSize = 0) 
		: base(todoTask => todoTask.Status == taskStatus)
	{
		ApplyOrderBy(todoTask => todoTask.Name);
		ApplyPaging(pageNumber, pageSize);
	}
}
