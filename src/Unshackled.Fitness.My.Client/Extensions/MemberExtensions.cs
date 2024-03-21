using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.My.Client.Extensions;

public static class MemberExtensions
{
	public static bool AreDefaultUnits(this Member member, UnitSystems units)
	{
		return member.Settings.DefaultUnits == units;
	}
}
