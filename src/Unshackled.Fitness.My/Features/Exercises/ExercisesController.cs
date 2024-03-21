using Microsoft.AspNetCore.Mvc;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;
using Unshackled.Fitness.My.Features.Exercises.Actions;

namespace Unshackled.Fitness.My.Features.Exercises;

[ApiController]
[ApiRoute("exercises")]
public class ExercisesController : BaseController
{
	[HttpPost("add")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Add([FromBody] FormExerciseModel model)
	{
		return Ok(await Mediator.Send(new AddExercise.Command(Member.Id, model)));
	}

	[HttpGet("get/{sid}")]
	[DecodeId]
	public async Task<IActionResult> GetExercise(long id)
	{
		return Ok(await Mediator.Send(new GetExercise.Query(Member.Id, id)));
	}

	[HttpPost("import")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Import([FromBody] List<LibraryListModel> exercises)
	{
		return Ok(await Mediator.Send(new ImportExercises.Command(Member.Id, exercises)));
	}

	[HttpGet("list/{sid}/records")]
	[DecodeId]
	public async Task<IActionResult> ListRecords(long id)
	{
		return Ok(await Mediator.Send(new ListPersonalRecords.Query(Member.Id, id)));
	}

	[HttpPost("merge")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Merge([FromBody] MergeModel model)
	{
		return Ok(await Mediator.Send(new MergeExercises.Command(Member.Id, model.KeptSid, model.DeletedSid)));
	}

	[HttpPost("merge/list")]
	[ActiveMemberRequired]
	public async Task<IActionResult> ListMergeModels([FromBody] List<string> uids)
	{
		return Ok(await Mediator.Send(new ListMergeExercises.Query(Member.Id, uids)));
	}

	[HttpPost("save-note")]
	[ActiveMemberRequired]
	public async Task<IActionResult> SaveExerciseNote([FromBody] ExerciseNoteModel model)
	{
		return Ok(await Mediator.Send(new SaveExerciseNote.Command(Member.Id, model)));
	}

	[HttpPost("search")]
	public async Task<IActionResult> Search([FromBody] SearchExerciseModel model)
	{
		return Ok(await Mediator.Send(new SearchExercises.Query(Member.Id, model)));
	}

	[HttpPost("search-results")]
	public async Task<IActionResult> SearchResults([FromBody] SearchResultsModel model)
	{
		return Ok(await Mediator.Send(new SearchResults.Query(Member.Id, model)));
	}

	[HttpPost("toggle/archived")]
	[ActiveMemberRequired]
	public async Task<IActionResult> ToggleArchived([FromBody] string sid)
	{
		return Ok(await Mediator.Send(new ToggleIsArchived.Command(Member.Id, sid)));
	}

	[HttpPost("update")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Update([FromBody] FormExerciseModel model)
	{
		return Ok(await Mediator.Send(new UpdateExercise.Command(Member.Id, model)));
	}
}
