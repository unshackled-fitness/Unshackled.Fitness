namespace Unshackled.Fitness.Core.Models;

public interface ISearchModel
{
	int Page { get; set; }
	int PageSize { get; set; }
	int Skip { get; }
	List<SearchSortModel> Sorts { get; set; }
}

public abstract class SearchModel : ISearchModel
{
	public const int DefaultPageSize = 25;
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = DefaultPageSize;
	public List<SearchSortModel> Sorts { get; set; } = new();
	public int Skip => Page > 1 ? (Page - 1) * PageSize : 0;
}
