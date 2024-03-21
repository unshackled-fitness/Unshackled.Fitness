namespace Unshackled.Fitness.Core.Configuration;

public static class HashIdSettings
{
	public static string Alphabet = string.Empty;
	public static string Salt = string.Empty;
	public static int MinLength;

	public static void Configure(string alphabet, string salt, int minLength)
	{
		Alphabet = alphabet;
		Salt = salt;
		MinLength = minLength;
	}
}
