using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.Core.Extensions;

public static class ListExtensions
{
	public static void MoveAndSort<T>(this List<T> list, int fromIndex, int toIndex) where T : ISortable
	{
		var item = list[fromIndex];
		list.RemoveAt(fromIndex);

		if (toIndex >= list.Count)
		{
			list.Add(item);
		}
		else
		{
			list.Insert(toIndex, item);
		}

		for (int i = 0; i < list.Count; i++)
		{
			list[i].SortOrder = i;
		}
	}
}
