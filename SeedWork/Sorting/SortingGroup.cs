namespace SeedWork.Sorting;

public record SortingGroup
{
	public required Dictionary<int, SortingRule> SortingRules { get; init; }
}
