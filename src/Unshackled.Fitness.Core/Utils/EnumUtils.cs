namespace Unshackled.Fitness.Core.Utils;

public static class EnumUtils
{
	public const string Separator = "|";

	public static List<T> FromJoinedIntString<T>(this string value)
	{
		string[] array = value.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
		return array.Select(x => (T)Enum.Parse(typeof(T), x)).ToList();
	}

	public static IEnumerable<T> GetValues<T>()
	{
		return Enum.GetValues(typeof(T)).Cast<T>();
	}

	public static List<T> GetSortedValues<T>()
	{
		return Enum.GetValues(typeof(T)).Cast<T>()
			.OrderBy(x => x!.ToString())
			.ToList();
	}
}