namespace Unshackled.Fitness.Core.Models;

public class SortableGroupResult<TGroup, TItem>
	where TGroup : ISortableGroup, new()
	where TItem : IGroupedSortable, new()
{
	public List<TGroup> DeletedGroups { get; set; } = new();
	public List<TGroup> Groups { get; set; } = new();
	public List<TItem> Items { get; set; } = new();
}
