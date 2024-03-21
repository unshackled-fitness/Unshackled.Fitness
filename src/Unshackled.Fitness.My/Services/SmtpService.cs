using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Unshackled.Fitness.Core.Configuration;
using Unshackled.Fitness.Core.Data.Entities;

namespace Unshackled.Fitness.My.Services;

public class SmtpService : IEmailSender<UserEntity>
{
	private readonly SmtpSettings smtpSettings;

	public SmtpService(SmtpSettings smtpSettings)
	{
		this.smtpSettings = smtpSettings;
	}

	public Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		return SendEmailAsync(email, string.Empty, subject, htmlMessage);
	}

	public Task SendEmailAsync(string email, string from, string subject, string htmlMessage)
	{
		var client = new SmtpClient(smtpSettings.Host, smtpSettings.Port)
		{
			Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password),
			EnableSsl = smtpSettings.UseSSL
		};
		var sender = string.IsNullOrEmpty(from) ? smtpSettings.DefaultReplyTo : from;
		if (!string.IsNullOrEmpty(sender))
		{
			return client.SendMailAsync(
				new MailMessage(sender, email, subject, htmlMessage) { IsBodyHtml = true }
			);
		}

		return Task.CompletedTask;
	}

	public Task SendConfirmationLinkAsync(UserEntity user, string email, string confirmationLink) =>
		SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

	public Task SendPasswordResetLinkAsync(UserEntity user, string email, string resetLink) =>
		SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

	public Task SendPasswordResetCodeAsync(UserEntity user, string email, string resetCode) =>
		SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
}
