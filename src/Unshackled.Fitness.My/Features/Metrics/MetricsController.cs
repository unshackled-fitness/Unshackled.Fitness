using Microsoft.AspNetCore.Mvc;
using Unshackled.Fitness.My.Client.Features.Metrics.Models;
using Unshackled.Fitness.My.Features.Metrics.Actions;

namespace Unshackled.Fitness.My.Features.Metrics;

[ApiController]
[ApiRoute("metrics")]
public class MetricsController : BaseController
{
	[HttpPost("delete")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Delete([FromBody] string sid)
	{
		return Ok(await Mediator.Send(new DeleteDefinition.Command(Member.Id, sid)));
	}

	[HttpGet("get/{sid}")]
	[DecodeId]
	public async Task<IActionResult> Get(long id)
	{
		return Ok(await Mediator.Send(new GetDefinition.Query(Member.Id, id)));
	}

	[HttpPost("get-calendar/{sid}")]
	[DecodeId]
	public async Task<IActionResult> GetCalendar(long id, [FromBody] SearchCalendarModel model)
	{
		return Ok(await Mediator.Send(new GetCalendar.Query(Member.Id, id, model)));
	}

	[HttpGet("list")]
	public async Task<IActionResult> List()
	{
		return Ok(await Mediator.Send(new ListMetrics.Query(Member.Id)));
	}

	[HttpPost("save")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Save([FromBody] FormMetricDefinitionModel model)
	{
		return Ok(await Mediator.Send(new SaveDefinition.Command(Member.Id, model)));
	}

	[HttpPost("toggle-archived/{sid}")]
	[ActiveMemberRequired]
	[DecodeId]
	public async Task<IActionResult> ToggleArchived(long id, [FromBody] bool isArchived)
	{
		return Ok(await Mediator.Send(new ToggleArchived.Command(Member.Id, id, isArchived)));
	}

	[HttpPost("update-sort")]
	[ActiveMemberRequired]
	public async Task<IActionResult> UpdateSort([FromBody] UpdateSortModel model)
	{
		return Ok(await Mediator.Send(new UpdateSort.Command(Member.Id, model)));
	}
}
