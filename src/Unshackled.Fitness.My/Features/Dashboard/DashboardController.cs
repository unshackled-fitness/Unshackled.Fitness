using Microsoft.AspNetCore.Mvc;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;
using Unshackled.Fitness.My.Features.Dashboard.Actions;

namespace Unshackled.Fitness.My.Features.Dashboard;

[ApiController]
[ApiRoute("dashboard")]
public class DashboardController : BaseController
{
	[HttpPost("add-workout")]
	[ActiveMemberRequired]
	public async Task<IActionResult> AddWorkout([FromBody] string templateSid)
	{
		return Ok(await Mediator.Send(new AddWorkout.Command(Member.Id, templateSid)));
	}

	[HttpPost("hide-getting-started")]
	public async Task<IActionResult> HideGettingStarted([FromBody] bool hide)
	{
		return Ok(await Mediator.Send(new HideGettingStarted.Command(Member.Id)));
	}

	[HttpPost("list-metrics")]
	public async Task<IActionResult> ListMetrics([FromBody] DateTime displayDateUtc)
	{
		return Ok(await Mediator.Send(new ListMetrics.Query(Member.Id, displayDateUtc)));
	}

	[HttpPost("list-program-items")]
	public async Task<IActionResult> ListProgramItems([FromBody] DateTime displayDateUtc)
	{
		return Ok(await Mediator.Send(new ListProgramItems.Query(Member.Id, displayDateUtc)));
	}

	[HttpPost("save-metric")]
	[ActiveMemberRequired]
	public async Task<IActionResult> SaveMetric([FromBody] SaveMetricModel model)
	{
		return Ok(await Mediator.Send(new SaveMetric.Command(Member.Id, model)));
	}

	[HttpPost("skip-workout")]
	[ActiveMemberRequired]
	public async Task<IActionResult> SkipWorkout([FromBody] string programSid)
	{
		return Ok(await Mediator.Send(new SkipWorkout.Command(Member.Id, programSid)));
	}

	[HttpPost("workout-stats")]
	public async Task<IActionResult> GetWorkoutStats([FromBody] DateTime toDateUtc)
	{
		return Ok(await Mediator.Send(new GetWorkoutStats.Query(Member.Id, toDateUtc)));
	}
}
