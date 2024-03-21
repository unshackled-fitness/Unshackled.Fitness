using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.My.Client.Features.Dashboard.Models;

public class ProgramListModel
{
	public string Sid { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public bool IsCompleted { get; set; } = false;
	public bool IsStarted { get; set; } = false;
	public string ProgramSid { get; set; } = string.Empty;
	public string ProgramTitle { get; set; } = string.Empty;
	public ProgramTypes ProgramType { get; set; }
}
