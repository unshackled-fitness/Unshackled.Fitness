namespace Unshackled.Fitness.Core.Web.Extensions;

public static class DecimalExtensions
{
	public static string ShortLabel(this decimal? value)
	{
		if (!value.HasValue)
			return "0";

		if (value.Value > 1000000000)
		{
			return (value.Value / 1000000000).ToString("0.#") + "B";
		}
		else if (value.Value > 1000000)
		{
			return (value.Value / 1000000).ToString("0.#") + "M";
		}
		else if (value.Value > 1000)
		{
			return (value.Value / 1000).ToString("0.#") + "K";
		}
		else
		{
			return value.Value.ToString("0.#");
		}
	}
}
