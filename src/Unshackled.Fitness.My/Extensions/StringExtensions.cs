using System.Security.Cryptography;
using System.Text;
using HashidsNet;
using Unshackled.Fitness.Core.Configuration;

namespace Unshackled.Fitness.My.Extensions;

public static class StringExtensions
{
	public static long DecodeLong(this string value)
	{
		try
		{
			var hashids = new Hashids(HashIdSettings.Salt, HashIdSettings.MinLength, HashIdSettings.Alphabet);
			return hashids.DecodeSingleLong(value);
		}
		catch (NoResultException)
		{
			return 0L;
		}
	}

	public static string ComputeSha256Hash(this string rawData)
	{
		// Create a SHA256
		using (SHA256 sha256Hash = SHA256.Create())
		{
			// ComputeHash - returns byte array
			byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

			// Convert byte array to a string
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
			{
				builder.Append(bytes[i].ToString("x2"));
			}
			return builder.ToString();
		}
	}
}