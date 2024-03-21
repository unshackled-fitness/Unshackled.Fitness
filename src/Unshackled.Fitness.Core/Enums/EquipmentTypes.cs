using Unshackled.Fitness.Core.Utils;

namespace Unshackled.Fitness.Core.Enums;

public enum EquipmentTypes
{
	Any = 0,
	Bands = 1,
	Barbell = 2,
	PullUpBar = 3,
	Cable = 4,
	Dumbbell = 5,
	EzCurlBar = 6,
	ExerciseBall = 7,
	FoamRoll = 8,
	KettleBell = 9,
	Machine = 10,
	MedicineBall = 11,
	None = 12,
	Other = 13,
	SmithMachine = 14,
	LegPress = 15,
	HipSled = 16,
	LegCurl = 17,
	LegExtension = 18,
	GHD = 19,
	GluteKickback = 20,
	CalfMachine = 21,
	DipRack = 22,
	BosuBall = 23,
	Box = 24,
	Straps = 25,
	AnkleCuff = 26,
	LatBar = 27,
	NeutralGripLatBar = 28,
	OpenRow = 29,
	Rope = 30,
	SingleCableHandle = 31,
	SportsHandle = 32,
	StraightBar = 33,
	TriangleRow = 34,
	TricepStrap = 35,
	VBar = 36,
	Landmine = 37,
	Chains = 38
}

public static class EquipmentTypesExtensions
{
	public static string Title(this EquipmentTypes equip)
	{
		return equip switch
		{
			EquipmentTypes.AnkleCuff => "Ankle Cuff",
			EquipmentTypes.Any => "All",
			EquipmentTypes.Bands => "Bands",
			EquipmentTypes.Barbell => "Barbell",
			EquipmentTypes.BosuBall => "Bosu Ball",
			EquipmentTypes.Box => "Box",
			EquipmentTypes.Cable => "Cable",
			EquipmentTypes.CalfMachine => "Calf Machine",
			EquipmentTypes.Chains => "Chains",
			EquipmentTypes.DipRack => "Dip Rack",
			EquipmentTypes.Dumbbell => "Dumbbell",
			EquipmentTypes.ExerciseBall => "Exercise Ball",
			EquipmentTypes.EzCurlBar => "E-Z Curl Bar",
			EquipmentTypes.FoamRoll => "Foam Roll",
			EquipmentTypes.GHD => "GHD",
			EquipmentTypes.GluteKickback => "Glute Kickback Machine",
			EquipmentTypes.HipSled => "Hip Sled",
			EquipmentTypes.KettleBell => "Kettlebell",
			EquipmentTypes.Landmine => "Landmine",
			EquipmentTypes.LatBar => "Lat Bar",
			EquipmentTypes.LegCurl => "Leg Curl Machine",
			EquipmentTypes.LegExtension => "Leg Extension Machine",
			EquipmentTypes.LegPress => "Leg Press",
			EquipmentTypes.Machine => "Machine",
			EquipmentTypes.MedicineBall => "Medicine Ball",
			EquipmentTypes.NeutralGripLatBar => "Neutral Grip Lat Bar",
			EquipmentTypes.None => "None",
			EquipmentTypes.OpenRow => "Open Row",
			EquipmentTypes.Other => "Other",
			EquipmentTypes.PullUpBar => "Pull-Up Bar",
			EquipmentTypes.Rope => "Rope Attachment",
			EquipmentTypes.SingleCableHandle => "Single Cable Handle",
			EquipmentTypes.SmithMachine => "Smith Machine",
			EquipmentTypes.SportsHandle => "Sports Handle",
			EquipmentTypes.StraightBar => "Straight Bar",
			EquipmentTypes.Straps => "Straps",
			EquipmentTypes.TriangleRow => "Triangle Row",
			EquipmentTypes.TricepStrap => "Tricep Strap",
			EquipmentTypes.VBar => "V-Bar",
			_ => string.Empty,
		};
	}

	public static string Titles(this IEnumerable<EquipmentTypes> equipmentTypes)
	{
		string equipment = string.Join(", ", equipmentTypes.Select(x => x.Title()).ToArray());
		if (equipment != EquipmentTypes.None.Title())
		{
			return equipment;
		}
		return string.Empty;
	}

	public static string ToJoinedIntString(this IEnumerable<EquipmentTypes> list)
	{
		int[] intArray = list.Select(x => (int)x).ToArray();
		return $"{EnumUtils.Separator}{String.Join(EnumUtils.Separator, intArray)}{EnumUtils.Separator}";
	}

	public static string ToSearchString(this EquipmentTypes equipmentType)
	{
		return $"{EnumUtils.Separator}{(int)equipmentType}{EnumUtils.Separator}";
	}
}