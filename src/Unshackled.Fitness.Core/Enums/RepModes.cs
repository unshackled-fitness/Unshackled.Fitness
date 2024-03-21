namespace Unshackled.Fitness.Core.Enums;

public enum RepModes
{
	Exact = 0,
	ToFailure = 1
}

public static class MetricModesExtensions
{
	public static string Label(this RepModes mode, int reps, int intensity = 0)
	{
		string level = string.Empty;
		if (intensity > 0)
		{
			level = $" @ {intensity}";
		}
		return mode switch
		{
			RepModes.Exact => $"Target: {reps} reps{level}",
			RepModes.ToFailure => $"To Failure{level}",
			_ => string.Empty
		};
	}

	public static string Title(this RepModes mode)
	{
		return mode switch
		{
			RepModes.Exact => "Exact",
			RepModes.ToFailure => "To Failure",
			_ => string.Empty
		};
	}
}

