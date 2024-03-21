namespace Unshackled.Fitness.My.Client.Features.Dashboard.Models;

public class MetricGridModel
{
	public List<MetricDefinitionGroupModel> Groups { get; set; } = new();
	public List<MetricModel> Metrics { get; set; } = new();
}
