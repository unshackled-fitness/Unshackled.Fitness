namespace Unshackled.Fitness.My.Client.Features.Programs.Models;

public class FormUpdateTemplatesModel
{
	public string ProgramSid { get; set; } = string.Empty;
	public int LengthWeeks { get; set; }
	public List<FormProgramTemplateModel> Templates { get; set; } = new();
	public List<FormProgramTemplateModel> DeletedTemplates { get; set; } = new();
}
