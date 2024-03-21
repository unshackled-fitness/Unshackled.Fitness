namespace Unshackled.Fitness.My.Client.Features.Members.Models;

public class AuthenticatorModel
{
	public const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

	public string? SharedKey { get; set; }
	public string? AuthenticatorUri { get; set; }
	public IEnumerable<string>? RecoveryCodes { get; set; }
}
