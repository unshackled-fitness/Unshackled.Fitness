namespace Unshackled.Fitness.Core.Enums;

public enum MetricTypes
{
	ExactValue = 0,
	Toggle = 1,
	Counter = 2,
	Range = 3
}

public static class MetricTypesExtensions
{
	public static string Title(this MetricTypes mode)
	{
		return mode switch
		{
			MetricTypes.ExactValue => "Exact Value",
			MetricTypes.Toggle => "Toggle",
			MetricTypes.Counter => "Counter",
			MetricTypes.Range => "Range",
			_ => string.Empty
		};
	}
}

