using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services.Specifications;

public sealed class TaskLastLogSpecification : BaseSpecification<TaskLog>
{
	public TaskLastLogSpecification(Guid taskId) : base(taskLog => taskLog.TaskId.Equals(taskId))
	{
		ApplyOrderByDescending(taskLog => taskLog.DateTime);
	}
}
