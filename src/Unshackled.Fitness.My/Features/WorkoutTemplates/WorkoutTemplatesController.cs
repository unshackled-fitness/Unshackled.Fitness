using Microsoft.AspNetCore.Mvc;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;
using Unshackled.Fitness.My.Features.WorkoutTemplates.Actions;

namespace Unshackled.Fitness.My.Features.WorkoutTemplates;

[ApiController]
[ApiRoute("templates")]
public class WorkoutTemplatesController : BaseController
{
	[HttpPost("add")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Add([FromBody] FormTemplateModel model)
	{
		return Ok(await Mediator.Send(new AddTemplate.Command(Member.Id, model)));
	}

	[HttpPost("add-workout")]
	[ActiveMemberRequired]
	public async Task<IActionResult> AddWorkout([FromBody] string templateSid)
	{
		return Ok(await Mediator.Send(new AddWorkout.Command(Member.Id, templateSid)));
	}

	[HttpPost("delete")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Delete([FromBody] string templateSid)
	{
		return Ok(await Mediator.Send(new DeleteTemplate.Command(Member.Id, templateSid)));
	}

	[HttpPost("duplicate/{sid}")]
	[ActiveMemberRequired]
	[DecodeId]
	public async Task<IActionResult> Duplicate(long id, [FromBody] FormTemplateModel model)
	{
		return Ok(await Mediator.Send(new DuplicateTemplate.Command(Member.Id, id, model)));
	}

	[HttpGet("get/{sid}")]
	[DecodeId]
	public async Task<IActionResult> Get(long id)
	{
		return Ok(await Mediator.Send(new GetTemplate.Query(Member.Id, id)));
	}

	[HttpGet("get/{sid}/groups")]
	[DecodeId]
	public async Task<IActionResult> GetSetGroups(long id)
	{
		return Ok(await Mediator.Send(new ListSetGroups.Query(Member.Id, id)));
	}

	[HttpGet("get/{sid}/sets")]
	[DecodeId]
	public async Task<IActionResult> GetSets(long id)
	{
		return Ok(await Mediator.Send(new ListSets.Query(Member.Id, id)));
	}

	[HttpGet("get/{sid}/tasks/{type:int}")]
	[DecodeId]
	public async Task<IActionResult> GetTasks(long id, int type)
	{
		WorkoutTaskTypes taskType = (WorkoutTaskTypes)type;
		return Ok(await Mediator.Send(new ListTasks.Query(Member.Id, id, taskType)));
	}

	[HttpPost("search")]
	public async Task<IActionResult> Search([FromBody] SearchTemplateModel model)
	{
		return Ok(await Mediator.Send(new SearchTemplates.Query(Member.Id, model)));
	}

	[HttpPost("update")]
	[ActiveMemberRequired]
	public async Task<IActionResult> UpdateProperties([FromBody] FormTemplateModel model)
	{
		return Ok(await Mediator.Send(new UpdateTemplateProperties.Command(Member.Id, model)));
	}

	[HttpPost("update/{sid}/sets")]
	[ActiveMemberRequired]
	[DecodeId]
	public async Task<IActionResult> UpdateSets(long id, [FromBody]UpdateTemplateSetsModel model)
	{
		return Ok(await Mediator.Send(new UpdateTemplateSets.Command(Member.Id, id, model)));
	}

	[HttpPost("update/{sid}/tasks")]
	[ActiveMemberRequired]
	[DecodeId]
	public async Task<IActionResult> UpdateTasks(long id, [FromBody] UpdateTemplateTasksModel model)
	{
		return Ok(await Mediator.Send(new UpdateTemplateTasks.Command(Member.Id, id, model)));
	}
}
