namespace Unshackled.Fitness.My.Client.Features.Metrics.Models;

public class UpdateSortModel
{
	public List<FormMetricDefinitionGroupModel> DeletedGroups { get; set; } = new();
	public List<FormMetricDefinitionGroupModel> Groups { get; set; } = new();
	public List<FormMetricDefinitionModel> Metrics { get; set; } = new();
}
