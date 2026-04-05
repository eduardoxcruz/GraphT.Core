namespace SeedWork;

public class PagedList<TEntity>
{
	public List<TEntity> Items { get; private set; }
	public int CurrentPage { get; private set; }
	public int TotalPages { get; private set; }
	public int PageSize { get; private set; }
	public int TotalCount { get; private set; }
	public bool HasPrevious => CurrentPage > 1;
	public bool HasNext => CurrentPage < TotalPages;

	public PagedList(List<TEntity> items, int totalCount, int pageNumber, int pageSize)
	{
		if (pageNumber < 1) pageNumber = 1;
		if (pageSize < 1) pageSize = PagingOptions.MaxPageSize;
		
		TotalCount = totalCount;
		PageSize = pageSize;
		CurrentPage = pageNumber;
		TotalPages = totalCount != 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 1;
		Items = new List<TEntity>(items);
	}
}
