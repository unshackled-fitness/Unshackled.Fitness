using Unshackled.Fitness.Core.Utils;

namespace Unshackled.Fitness.Core.Enums;

public enum MuscleTypes
{
	Any = 0,
	Abdominals = 1,
	Abductors = 2,
	Adductors = 3,
	Biceps = 4,
	Calves = 5,
	Pectorals = 6,
	Forearms = 7,
	Glutes = 8,
	Hamstrings = 9,
	Lats = 10,
	LowerBack = 11,
	MiddleBack = 12,
	Neck = 13,
	Quadriceps = 14,
	Deltoids = 15,
	Traps = 16,
	Triceps = 17,
	None = 18
}

public static class MuscleTypesExtensions
{
	public static string Title(this MuscleTypes muscle)
	{
		return muscle switch
		{
			MuscleTypes.Any => "All",
			MuscleTypes.Abdominals => "Abdominals",
			MuscleTypes.Abductors => "Abductors",
			MuscleTypes.Adductors => "Adductors",
			MuscleTypes.Biceps => "Biceps",
			MuscleTypes.Calves => "Calves",
			MuscleTypes.Pectorals => "Pectorals",
			MuscleTypes.Forearms => "Forearms",
			MuscleTypes.Glutes => "Glutes",
			MuscleTypes.Hamstrings => "Hamstrings",
			MuscleTypes.Lats => "Lats",
			MuscleTypes.LowerBack => "Lower Back",
			MuscleTypes.MiddleBack => "Middle Back",
			MuscleTypes.Neck => "Neck",
			MuscleTypes.Quadriceps => "Quadriceps",
			MuscleTypes.Deltoids => "Deltoids",
			MuscleTypes.Traps => "Traps",
			MuscleTypes.Triceps => "Triceps",
			MuscleTypes.None => "None",
			_ => string.Empty,
		};
	}

	public static string Titles(this IEnumerable<MuscleTypes> muscleTypes)
	{
		return string.Join(", ", muscleTypes.Select(x => x.Title()).ToArray());
	}

	public static string ToJoinedIntString(this IEnumerable<MuscleTypes> list)
	{
		int[] intArray = list.Select(x => (int)x).ToArray();
		return $"{EnumUtils.Separator}{String.Join(EnumUtils.Separator, intArray)}{EnumUtils.Separator}";
	}

	public static string ToSearchString(this MuscleTypes muscleType)
	{
		return $"{EnumUtils.Separator}{(int)muscleType}{EnumUtils.Separator}";
	}
}
