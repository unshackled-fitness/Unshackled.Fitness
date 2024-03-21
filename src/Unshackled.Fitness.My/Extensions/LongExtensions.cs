using HashidsNet;
using Unshackled.Fitness.Core.Configuration;

namespace Unshackled.Fitness.My.Extensions;

public static class LongExtensions
{
	public static string Encode(this long value)
	{
		var hashids = new Hashids(HashIdSettings.Salt, HashIdSettings.MinLength, HashIdSettings.Alphabet);
		return hashids.EncodeLong(value);
	}
}