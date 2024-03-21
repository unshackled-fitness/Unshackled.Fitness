namespace Unshackled.Fitness.Core.Configuration;

public class SiteConfiguration
{
	public string? SiteName { get; set; }
	public string AppThemeColor { get; set; } = string.Empty;
	public bool AllowRegistration { get; set; } = true;
	public bool RequireConfirmedAccount { get; set; } = true;

	public PasswordStrength PasswordStrength { get; set; } = new();
}

public class PasswordStrength
{
	public bool RequireDigit { get; set; } = true;
	public bool RequireLowercase { get; set; } = true;
	public bool RequireNonAlphanumeric { get; set; } = true;
	public bool RequireUppercase { get; set; } = true;
	public int RequiredLength { get; set; } = 6;
	public int RequiredUniqueChars { get; set; } = 1;
}