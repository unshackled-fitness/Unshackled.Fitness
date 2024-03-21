namespace Unshackled.Fitness.My.Client.Features.Metrics.Models;

public class MetricListModel
{
	public List<FormMetricDefinitionGroupModel> Groups { get; set; } = new();
	public List<FormMetricDefinitionModel> Metrics { get; set; } = new();
}
