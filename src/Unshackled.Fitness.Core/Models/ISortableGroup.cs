namespace Unshackled.Fitness.Core.Models;

public interface ISortableGroup : ISortable
{
	string Sid { get; set; }
	string Title { get; set; }
}
