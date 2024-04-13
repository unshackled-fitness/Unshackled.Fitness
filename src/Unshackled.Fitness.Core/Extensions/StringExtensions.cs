using System.Text;
using System.Text.RegularExpressions;

namespace Unshackled.Fitness.Core.Extensions;

public static class StringExtensions
{
	public static string? FormatUrl(this string? url)
	{
		string? newUrl = url?.Trim();
		if (!string.IsNullOrEmpty(newUrl) && !newUrl.StartsWith("http://") && !newUrl.StartsWith("https://"))
			newUrl = $"https://{newUrl}";

		return newUrl;
	}

	public static string NormalizeKey(this string? value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return value.Trim().ToLower().ReplaceNonAlphaNumeric('-').Trim('-');
		}
		return string.Empty;
	}

	public static string ReplaceLineBreaks(this string? value, string replacement)
	{
		if (!string.IsNullOrEmpty(value))
		{
			string result = Regex.Replace(value, @"\\r\\n?|\\n", replacement);
			return Regex.Replace(result, @"\r\n?|\n", replacement);
		}
		return string.Empty;
	}

	public static string ReplaceNonAlphaNumeric(this string? value, char replacement)
	{
		if (!string.IsNullOrEmpty(value))
		{
			var sb = new StringBuilder();
			char lastChar = replacement;
			foreach (char c in value)
			{
				if (c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z')
				{
					sb.Append(c);
					lastChar = c;
				}
				else
				{
					if (lastChar != replacement) sb.Append(replacement);
					lastChar = replacement;
				}
			}
			return sb.ToString();
		}
		return string.Empty;
	}

	public static string ReplaceSpaces(this string? value, char replacement)
	{
		if (!string.IsNullOrEmpty(value))
		{
			var sb = new StringBuilder();
			char lastChar = replacement;
			foreach (char c in value)
			{
				if (c != ' ')
				{
					sb.Append(c);
					lastChar = c;
				}
				else
				{
					if (lastChar != replacement) sb.Append(replacement);
					lastChar = replacement;
				}
			}
			return sb.ToString();
		}
		return string.Empty;
	}

	public static string RemoveNonAlphaNumeric(this string value)
	{
		var sb = new StringBuilder();
		foreach (char c in value)
		{
			if (c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z')
			{
				sb.Append(c);
			}
		}
		return sb.ToString();
	}

	public static string RemoveNonAlphaNumeric(this string? value, char[] allowedChars)
	{
		if (!string.IsNullOrEmpty(value))
		{
			var sb = new StringBuilder();
			foreach (char c in value)
			{
				if (c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || allowedChars.Contains(c))
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}
		return string.Empty;
	}

	public static string RemoveNonNumeric(this string? value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			var sb = new StringBuilder();
			foreach (char c in value)
			{
				if (c >= '0' && c <= '9')
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}
		return string.Empty;
	}

	public static int[] SplitToIntArray(this string value, char separator)
	{
		return value.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToArray();
	}

	public static long[] SplitToLongArray(this string value, char separator)
	{
		return value.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToArray();
	}

	public static string ToShortString(this string? value, int length, string moreIndicator = "")
	{
		if (string.IsNullOrEmpty(value))
			return string.Empty;

		if (value.Length > length)
			return value.Substring(0, length) + moreIndicator;
		return value;
	}

}