using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.Model.Services.Specifications;

public sealed class TaskIncludeLifeAreasSpecification : BaseSpecification<TaskAggregate>
{
	public TaskIncludeLifeAreasSpecification(Guid id) : base(t => t.Id.Equals(id))
	{
		AddInclude(t => t.LifeAreas);
	}
}

