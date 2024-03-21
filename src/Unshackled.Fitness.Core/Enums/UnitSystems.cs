namespace Unshackled.Fitness.Core.Enums;

public enum UnitSystems
{
	None = 0,
	Metric = 1,
	Imperial = 2
}

public static class UnitSystemsExtensions
{
	public static string Title(this UnitSystems system)
	{
		return system switch
		{
			UnitSystems.None => "Universal",
			UnitSystems.Metric => "Metric",
			UnitSystems.Imperial => "US/Imperial",
			_ => string.Empty,
		};
	}
}
