using SeedWork.Filtering.Operators;

namespace SeedWork.Filtering;

public record FilterGroup
{
	public LogicalOperator GroupLogicalOperator { get; init; }
	public List<FilterGroup>? Groups { get; init; }
	public List<FilterRule>? Rules { get; init; }
}
