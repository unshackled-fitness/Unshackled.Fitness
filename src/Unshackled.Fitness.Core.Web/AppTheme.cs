using MudBlazor;

namespace Unshackled.Fitness.Core.Web;

public class AppTheme : MudTheme
{
	public AppTheme()
	{
		Palette = new PaletteLight()
		{
			Primary = "#010101",
			PrimaryContrastText = "#ffffff",
			Secondary = "#1161D1",
			SecondaryContrastText = "#ffffff",
			Tertiary = "#666666",
			Success = "#43C900",
			Error = "#E64A19",
			Info = "#039BE5",
			Warning = "#FF9800",
			WarningContrastText = "#FFFFFF",
			Surface = "#ffffff",
			AppbarBackground = "#ffffff",
			AppbarText = "#000000",
			Background = "#fefefe",
			BackgroundGrey = "#ececec",
			Dark = "#e6e6e6",
			DarkContrastText = "#666666",
			Divider = "#dfdfdf",
			DrawerBackground = "#ffffff",
			DrawerText = "#222222",
			DrawerIcon = "#373737",
			TextPrimary = "#373737",
			TextSecondary = "#757575",
			TextDisabled = "#424242",
			ActionDisabled = "#999999",
			TableLines = "#dfdfdf",
			TableHover = "#f9f9f9",
			TableStriped = "#fafafa",
			LinesDefault = "#dfdfdf",
			LinesInputs = "#cccccc",
			HoverOpacity = 0.1
		};

		PaletteDark = new PaletteDark()
		{
			Primary = "#F3F3F3",
			PrimaryContrastText = "#000000",
			Secondary = "#5292FF",
			SecondaryContrastText = "#FFFFFF",
			Tertiary = "#888888",
			Success = "#3DB700",
			Error = "#D84315",
			Info = "#29B6F6",
			Warning = "#FFB638",
			WarningContrastText = "#000000",
			Surface = "#252525",
			AppbarBackground = "#222222",
			AppbarText = "#ffffff",
			Black = "#000000",
			Background = "#202020",
			BackgroundGrey = "#303030",
			Dark = "#404040",
			DarkContrastText = "#e9e9e9",
			Divider = "#353535",
			DrawerBackground = "#222222",
			DrawerText = "#DFDFDF",
			DrawerIcon = "#C8C8C8",
			TextPrimary = "#C8C8C8",
			TextSecondary = "#808080",
			TextDisabled = "#666666",
			ActionDefault = "#aaaaaa",
			ActionDisabled = "#666666",
			ActionDisabledBackground = "#303030",
			TableLines = "#353535",
			TableHover = "#303030",
			TableStriped = "#2a2a2a",
			LinesDefault = "#353535",
			LinesInputs = "#505050",
			HoverOpacity = 0.2,
		};

		Typography = new Typography
		{
			Default = new Default
			{
				FontFamily = new string[] { "Roboto", "Helvetica", "San-Serif" },
				FontSize = "1rem",
				FontWeight = 400,
				LineHeight = 1.1
			},
			H1 = new H1
			{
				FontWeight = 300
			},
			H2 = new H2
			{
				FontWeight = 300
			},
			H3 = new H3
			{
				FontWeight = 300
			},
			H4 = new H4
			{
				FontWeight = 300
			},
			H5 = new H5
			{
				FontWeight = 300
			},
			H6 = new H6
			{
				FontWeight = 400
			},
			Subtitle1 = new Subtitle1
			{
				FontSize = "1.2rem",
				LineHeight = 1.2
			},
			Subtitle2 = new Subtitle2
			{
				FontSize = ".9rem",
				LineHeight = 1
			}
		};
	}
}
