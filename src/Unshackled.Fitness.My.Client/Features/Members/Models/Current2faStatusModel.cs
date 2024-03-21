namespace Unshackled.Fitness.My.Client.Features.Members.Models;

public class Current2faStatusModel
{
	public bool CanTrack { get; set; }
	public bool HasAuthenticator { get; set; }
	public int RecoveryCodesLeft { get; set; }
	public bool Is2faEnabled { get; set; }
	public bool IsMachineRemembered { get; set; }
}
