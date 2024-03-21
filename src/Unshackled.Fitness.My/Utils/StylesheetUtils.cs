namespace Unshackled.Fitness.My.Utils;

public static class StylesheetUtils
{

	public static string PrintRoot(string themeColor)
	{
		return $":root {{--mud-palette-background: {themeColor}; --mud-palette-text-primary: #ffffff; --uf-loading-background: {themeColor}; --uf-loading-text: #ffffff; }}";
	}
}
