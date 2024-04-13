using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Extensions;

public static class MemberExtensions
{
	public static bool AreDefaultUnits(this Member member, UnitSystems units)
	{
		return member.Settings.DefaultUnits == units;
	}
}
