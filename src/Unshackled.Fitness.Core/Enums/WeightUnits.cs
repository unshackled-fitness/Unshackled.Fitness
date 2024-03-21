namespace Unshackled.Fitness.Core.Enums;

public enum WeightUnits
{
	Any = 0,
	kg = 1,
	lb = 2
}

public static class MeasurementUnitsExtensions
{
	public static decimal ConversionFactor(this WeightUnits unit)
	{
		return unit switch
		{
			WeightUnits.lb => 2.2046M, // To Kg
			WeightUnits.kg => 1M,
			_ => 1M,
		};
	}

	public static string Label(this WeightUnits unit)
	{
		return unit switch
		{
			WeightUnits.Any => "Any",
			WeightUnits.kg => "kg",
			WeightUnits.lb => "lb",
			_ => string.Empty,
		};
	}
}
