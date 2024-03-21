using MudBlazor;

namespace Unshackled.Fitness.Core.Web.Extensions;

public static class EnumExtensions
{
	public static string MakeCssClass(this Breakpoint breakpoint, string prefix)
	{
		return breakpoint switch
		{
			Breakpoint.Xs => $"{prefix}-xs",
			Breakpoint.Sm => $"{prefix}-sm",
			Breakpoint.Md => $"{prefix}-md",
			Breakpoint.Lg => $"{prefix}-lg",
			Breakpoint.Xl => $"{prefix}-xl",
			Breakpoint.Xxl => $"{prefix}-xxl",
			Breakpoint.SmAndDown => $"{prefix}-sm-down",
			Breakpoint.MdAndDown => $"{prefix}-md-down",
			Breakpoint.LgAndDown => $"{prefix}-lg-down",
			Breakpoint.XlAndDown => $"{prefix}-xl-down",
			Breakpoint.SmAndUp => $"{prefix}-sm-up",
			Breakpoint.MdAndUp => $"{prefix}-md-up",
			Breakpoint.LgAndUp => $"{prefix}-lg-up",
			Breakpoint.XlAndUp => $"{prefix}-xl-up",
			Breakpoint.None => string.Empty,
			Breakpoint.Always => $"{prefix}",
			_ => string.Empty,
		};
	}
}

