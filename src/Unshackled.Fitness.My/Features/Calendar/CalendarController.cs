using Microsoft.AspNetCore.Mvc;
using Unshackled.Fitness.My.Client.Features.Calendar.Models;
using Unshackled.Fitness.My.Features.Calendar.Actions;

namespace Unshackled.Fitness.My.Features.Calendar;

[ApiController]
[ApiRoute("calendar")]
public class CalendarController : BaseController
{
	[HttpPost("add-preset")]
	[ActiveMemberRequired]
	public async Task<IActionResult> AddPreset([FromBody] FormPresetModel model)
	{
		return Ok(await Mediator.Send(new AddPreset.Command(Member.Id, model)));
	}

	[HttpPost("delete-preset")]
	[ActiveMemberRequired]
	public async Task<IActionResult> DeletePreset([FromBody] string sid)
	{
		return Ok(await Mediator.Send(new DeletePreset.Command(Member.Id, sid)));
	}

	[HttpPost("get")]
	public async Task<IActionResult> Get([FromBody] SearchCalendarModel model)
	{
		return Ok(await Mediator.Send(new GetCalendar.Query(Member.Id, model)));
	}

	[HttpGet("list-presets")]
	public async Task<IActionResult> ListPresets()
	{
		return Ok(await Mediator.Send(new ListPresets.Query(Member.Id)));
	}

	[HttpPost("update-preset/{sid}")]
	[ActiveMemberRequired]
	[DecodeId]
	public async Task<IActionResult> UpdatePreset(long id, [FromBody] string settings)
	{
		return Ok(await Mediator.Send(new UpdatePreset.Command(Member.Id, id, settings)));
	}
}
