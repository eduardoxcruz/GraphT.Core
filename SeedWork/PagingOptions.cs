namespace SeedWork;

public class PagingOptions
{
	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = 50;
	public const int MaxPageSize = 100;
}
