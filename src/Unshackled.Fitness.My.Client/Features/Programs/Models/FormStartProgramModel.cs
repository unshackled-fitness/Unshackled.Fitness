namespace Unshackled.Fitness.My.Client.Features.Programs.Models;

public class FormStartProgramModel
{
	public string Sid { get; set; } = string.Empty;
	public DateTime DateStart { get; set; }
	public int StartingTemplateIndex { get; set; }
}
