namespace SeedWork;

public class PagedList<TEntity> : List<TEntity>
{
	public int CurrentPage { get; private set; }
	public int TotalPages { get; private set; }
	public int PageSize { get; private set; }
	public int TotalCount { get; private set; }
	public bool HasPrevious => CurrentPage > 1;
	public bool HasNext => CurrentPage < TotalPages;

	public PagedList(List<TEntity> items, int count, int pageNumber, int pageSize)
	{
		if (pageNumber < 1) pageNumber = 1;
		if (pageSize < 1) pageSize = PagingOptions.MaxPageSize;
		
		TotalCount = count;
		PageSize = pageSize;
		CurrentPage = pageNumber;
		TotalPages = count != 0 ? (int)Math.Ceiling(count / (double)pageSize) : 1;
		AddRange(items);
	}

	public static PagedList<TEntity> ToPagedList(IQueryable<TEntity> source, int pageNumber, int pageSize)
	{
		if (pageNumber < 1) pageNumber = 1;
		if (pageSize < 1) pageSize = PagingOptions.MaxPageSize;
		
		int count = source.Count();
		List<TEntity> items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
		return new PagedList<TEntity>(items, count, pageNumber, pageSize);
	}
}
