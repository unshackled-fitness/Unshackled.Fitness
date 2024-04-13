namespace Unshackled.Fitness.Core.Models;

public class SearchResult<T> where T : class
{
	public SearchResult()
	{
		PageSize = SearchModel.DefaultPageSize;
	}

	public SearchResult(int pageSize)
	{
		PageSize = pageSize;
	}

	public List<T> Data { get; set; } = new();
	public int Total { get; set; } = 0;
	public int PageSize { get; set; }
}
