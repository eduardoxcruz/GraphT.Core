using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services.Specifications;

public sealed class TasksWithSpecificStatusSpecification : BaseSpecification<TodoTask>
{
	public TasksWithSpecificStatusSpecification(Status taskStatus) : base(todoTask => todoTask.Status == taskStatus)
	{
		ApplyOrderBy(todoTask => todoTask.Name);
	}
}
