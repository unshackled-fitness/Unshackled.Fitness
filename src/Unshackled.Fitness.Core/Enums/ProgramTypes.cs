namespace Unshackled.Fitness.Core.Enums;

public enum ProgramTypes
{
	Any = 0,
	FixedRepeating = 1,
	Sequential = 2
}

public static class ProgramTypesExtensions
{
	public static string Title(this ProgramTypes programTypes)
	{
		return programTypes switch
		{
			ProgramTypes.Any => "Any",
			ProgramTypes.FixedRepeating => "Fixed Day, Repeating Weeks",
			ProgramTypes.Sequential => "Sequential Days",
			_ => string.Empty,
		};
	}
}