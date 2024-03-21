using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.Core;

public class ListGroupModel : ISortableGroup
{
	public string Sid { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public int SortOrder { get; set; }
}
