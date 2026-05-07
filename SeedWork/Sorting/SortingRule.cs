namespace SeedWork.Sorting;

public record SortingRule
{
	public string Property { get; set; } = string.Empty;
	public SortingOrder Order { get; set; }
}
