using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Features.Members.Models;
using Unshackled.Fitness.My.Extensions;
using Unshackled.Fitness.My.Features.Members.Actions;

namespace Unshackled.Fitness.My.Features.Members;

[ApiController]
[ApiRoute("members")]
public class MembersController : BaseController
{
	[HttpGet("active")]
	public async Task<IActionResult> GetActive()
	{
		string email = User.GetEmailClaim();
		return Ok(await Mediator.Send(new GetMemberByEmail.Query(email.ToLower())));
	}

	[HttpPost("add-authenticator")]
	public async Task<IActionResult> AddAuthenticator([FromBody] FormAuthenticatorModel model)
	{
		return Ok(await Mediator.Send(new AddAuthenticator.Command(Request.HttpContext.User, model)));
	}

	[HttpPost("disable-2fa")]
	public async Task<IActionResult> Disable2fa()
	{
		return Ok(await Mediator.Send(new Disable2fa.Command(Request.HttpContext.User)));
	}

	[HttpPost("generate-recovery-codes")]
	public async Task<IActionResult> GenerateRecoveryCodes()
	{
		return Ok(await Mediator.Send(new GenerateRecoveryCodes.Command(Request.HttpContext.User)));
	}

	[HttpGet("get-authenticator-model")]
	public async Task<IActionResult> GetAuthenticatorModel()
	{
		return Ok(await Mediator.Send(new GetAuthenticatorModel.Query(Request.HttpContext.User)));
	}

	[HttpGet("get-current-2fa-status")]
	public async Task<IActionResult> GetCurrent2faStatus()
	{
		var trackingConsent = Request.HttpContext.Features.Get<ITrackingConsentFeature>();
		return Ok(await Mediator.Send(new GetCurrent2faStatus.Query(Request.HttpContext.User, trackingConsent)));
	}

	[HttpGet("get-current-user-email")]
	public async Task<IActionResult> GetCurrentUserEmailModel()
	{
		return Ok(await Mediator.Send(new GetCurrentUserEmail.Query(Request.HttpContext.User)));
	}

	[HttpPost("resend-verification-email")]
	public async Task<IActionResult> ResendVerificationEmail([FromBody] string url)
	{
		return Ok(await Mediator.Send(new ResendVerificationEmail.Command(Request.HttpContext.User, url)));
	}

	[HttpPost("reset-authenticator-key")]
	public async Task<IActionResult> ResetAuthenticatorKey()
	{
		return Ok(await Mediator.Send(new ResetAuthenticatorKey.Command(Request.HttpContext.User)));
	}

	[HttpPost("save-settings")]
	public async Task<IActionResult> SaveSettings([FromBody] AppSettings settings)
	{
		return Ok(await Mediator.Send(new SaveSettings.Command(Member.Id, settings)));
	}

	[HttpPost("set-theme")]
	public async Task<IActionResult> SetTheme([FromBody] Themes theme)
	{
		return Ok(await Mediator.Send(new SetTheme.Command(Member.Id, theme)));
	}

	[HttpPost("update-email")]
	public async Task<IActionResult> UpdateEmail([FromBody] FormChangeEmailModel model)
	{
		return Ok(await Mediator.Send(new UpdateEmail.Command(Request.HttpContext.User, model)));
	}

	[HttpPost("update-password")]
	public async Task<IActionResult> UpdatePassword([FromBody] FormChangePasswordModel model)
	{
		return Ok(await Mediator.Send(new UpdatePassword.Command(Request.HttpContext.User, model)));
	}
}
