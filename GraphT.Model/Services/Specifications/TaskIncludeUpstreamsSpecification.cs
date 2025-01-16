using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.Model.Services.Specifications;

public sealed class TaskIncludeUpstreamsSpecification : BaseSpecification<TodoTask>
{
	public TaskIncludeUpstreamsSpecification(Guid id) : base(t => t.Id.Equals(id))
	{
		AddInclude(t => t.Upstreams);
	}
}

