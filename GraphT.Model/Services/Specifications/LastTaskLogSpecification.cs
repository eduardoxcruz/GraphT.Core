using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services.Specifications;

public sealed class LastTaskLogSpecification : BaseSpecification<TaskLog>
{
	public LastTaskLogSpecification(Guid taskId) : base(taskLog => taskLog.TaskId.Equals(taskId))
	{
		ApplyOrderByDescending(taskLog => taskLog.DateTime);
	}
}
