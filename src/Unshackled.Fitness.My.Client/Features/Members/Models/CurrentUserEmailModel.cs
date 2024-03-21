namespace Unshackled.Fitness.My.Client.Features.Members.Models;

public class CurrentUserEmailModel
{
	public string Email { get; set; } = string.Empty;
	public bool IsEmailVerified { get; set; }
}
