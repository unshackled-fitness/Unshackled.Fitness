using Microsoft.AspNetCore.Mvc;
using Unshackled.Fitness.My.Client.Features.Programs.Models;
using Unshackled.Fitness.My.Features.Programs.Actions;

namespace Unshackled.Fitness.My.Features.Programs;

[ApiController]
[ApiRoute("programs")]
public class ProgramsController : BaseController
{
	[HttpPost("add")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Add([FromBody] FormAddProgramModel model)
	{
		return Ok(await Mediator.Send(new AddProgram.Command(Member.Id, model)));
	}

	[HttpPost("delete")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Delete([FromBody] string sid)
	{
		return Ok(await Mediator.Send(new DeleteProgram.Command(Member.Id, sid)));
	}

	[HttpGet("get/{sid}")]
	[DecodeId]
	public async Task<IActionResult> Get(long id)
	{
		return Ok(await Mediator.Send(new GetProgram.Query(Member.Id, id)));
	}

	[HttpGet("list-templates")]
	public async Task<IActionResult> ListTemplates()
	{
		return Ok(await Mediator.Send(new ListTemplates.Query(Member.Id)));
	}

	[HttpPost("save-templates")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Save([FromBody] FormUpdateTemplatesModel model)
	{
		return Ok(await Mediator.Send(new SaveTemplates.Command(Member.Id, model)));
	}

	[HttpPost("search")]
	public async Task<IActionResult> Search([FromBody] SearchProgramModel model)
	{
		return Ok(await Mediator.Send(new SearchPrograms.Query(Member.Id, model)));
	}

	[HttpPost("start")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Start([FromBody] FormStartProgramModel model)
	{
		return Ok(await Mediator.Send(new StartProgram.Command(Member.Id, model)));
	}

	[HttpPost("stop")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Stop([FromBody] string id)
	{
		return Ok(await Mediator.Send(new StopProgram.Command(Member.Id, id)));
	}

	[HttpPost("update")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Update([FromBody] FormUpdateProgramModel model)
	{
		return Ok(await Mediator.Send(new UpdateProperties.Command(Member.Id, model)));
	}
}
