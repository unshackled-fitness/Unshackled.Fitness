using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.Core.Models;

public class AppSettings : ICloneable
{
	public Themes AppTheme { get; set; } = Themes.System;
	public UnitSystems DefaultUnits { get; set; } = UnitSystems.Metric;
	public int DisplaySplitTracking { get; set; } = 0;
	public bool HideCompleteSets { get; set; } = false;
	public bool HideGettingStarted { get; set; } = false;
	public bool UseGravatar { get; set; } = false;

	public object Clone()
	{
		return new AppSettings
		{
			AppTheme = AppTheme,
			DefaultUnits = DefaultUnits,
			DisplaySplitTracking = DisplaySplitTracking,
			HideCompleteSets = HideCompleteSets,
			HideGettingStarted = HideGettingStarted,
			UseGravatar = UseGravatar
		};
	}
}
