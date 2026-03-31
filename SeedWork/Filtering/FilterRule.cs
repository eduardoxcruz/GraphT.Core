using SeedWork.Filtering.Operators;

namespace SeedWork.Filtering;

public record FilterRule
{
	public LogicalOperator? LogicalOperator { get; init; }
	public string Property { get; init; } = string.Empty;
	public string Value { get; init; } = string.Empty;
	public string? Value2 { get; init; }
	public TextFilterOperator? TextFilterOperator { get; init; }
	public NumberFilterOperator? NumberFilterOperator { get; init; }
	public BooleanFilterOperator? BooleanFilterOperator { get; init; }
	public SpecificDateOperator? SpecificDateOperator { get; init; }
	public DateRelativeToTodayFilterOperator? DateRelativeToTodayFilterOperator { get; init; }
	public SelectFilterOperator? SelectFilterOperator { get; init; }
	public RelationshipFilterOperator? RelationshipFilterOperator { get; init; }
}
