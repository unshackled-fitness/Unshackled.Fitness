using Microsoft.AspNetCore.Mvc;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Features.Workouts.Actions;

namespace Unshackled.Fitness.My.Features.Workouts;

[ApiController]
[ApiRoute("workouts")]
public class WorkoutsController : BaseController
{
	[HttpPost("add")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Add([FromBody] string workoutSid)
	{
		return Ok(await Mediator.Send(new AddWorkout.Command(Member.Id, workoutSid)));
	}

	[HttpPost("add-set")]
	[ActiveMemberRequired]
	public async Task<IActionResult>AddSet([FromBody] FormWorkoutSetModel model)
	{
		return Ok(await Mediator.Send(new AddSet.Command(Member.Id, model)));
	}
	[HttpPost("add-template")]
	[ActiveMemberRequired]
	public async Task<IActionResult> AddTemplate([FromBody] FormAddTemplateModel model)
	{
		return Ok(await Mediator.Send(new AddTemplate.Command(Member.Id, model)));
	}

	[HttpPost("delete")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Delete([FromBody] string sid)
	{
		return Ok(await Mediator.Send(new DeleteWorkout.Command(Member.Id, sid)));
	}

	[HttpPost("delete-incomplete-sets")]
	[ActiveMemberRequired]
	public async Task<IActionResult> DeleteIncompleteSets([FromBody] string sid)
	{
		return Ok(await Mediator.Send(new DeleteIncompleteSets.Command(Member.Id, sid)));
	}

	[HttpPost("delete-set")]
	[ActiveMemberRequired]
	public async Task<IActionResult> DeleteSet([FromBody] string sid)
	{
		return Ok(await Mediator.Send(new DeleteSet.Command(Member.Id, sid)));
	}

	[HttpPost("duplicate-set")]
	[ActiveMemberRequired]
	public async Task<IActionResult> DuplicateSet([FromBody] FormWorkoutSetModel model)
	{
		return Ok(await Mediator.Send(new DuplicateSet.Command(Member.Id, model)));
	}

	[HttpPost("finish")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Finish([FromBody] CompleteWorkoutModel model)
	{
		return Ok(await Mediator.Send(new CompleteWorkout.Command(Member.Id, model)));
	}

	[HttpGet("get/{sid}")]
	[DecodeId]
	public async Task<IActionResult> Get(long id)
	{
		return Ok(await Mediator.Send(new GetWorkout.Query(Member.Id, id)));
	}

	[HttpPost("save-note")]
	[ActiveMemberRequired]
	public async Task<IActionResult> SaveExerciseNote([FromBody] ExerciseNoteModel model)
	{
		return Ok(await Mediator.Send(new SaveExerciseNote.Command(Member.Id, model)));
	}

	[HttpPost("save-set")]
	[ActiveMemberRequired]
	public async Task<IActionResult> SaveSet([FromBody] FormWorkoutSetModel set)
	{
		return Ok(await Mediator.Send(new SaveSet.Command(Member.Id, set)));
	}

	[HttpPost("search")]
	public async Task<IActionResult> Search([FromBody] SearchWorkoutModel model)
	{
		return Ok(await Mediator.Send(new SearchWorkouts.Query(Member.Id, model)));
	}

	[HttpPost("search-completed-sets")]
	public async Task<IActionResult> SearchCompletedSets([FromBody] SearchSetModel model)
	{
		return Ok(await Mediator.Send(new SearchCompletedSets.Query(Member.Id, model)));
	}

	[HttpPost("start")]
	[ActiveMemberRequired]
	public async Task<IActionResult> Start([FromBody] string workoutSid)
	{
		return Ok(await Mediator.Send(new StartWorkout.Command(Member.Id, workoutSid)));
	}

	[HttpPost("update-properties")]
	[ActiveMemberRequired]
	public async Task<IActionResult> UpdateProperties([FromBody] FormPropertiesModel model)
	{
		return Ok(await Mediator.Send(new UpdateProperties.Command(Member.Id, model)));
	}

	[HttpPost("update-set-properties")]
	[ActiveMemberRequired]
	public async Task<IActionResult> UpdateSetProperties([FromBody] FormWorkoutSetModel set)
	{
		return Ok(await Mediator.Send(new UpdateSetProperties.Command(Member.Id, set)));
	}

	[HttpPost("update-set-sorts")]
	[ActiveMemberRequired]
	public async Task<IActionResult> UpdateSetSorts([FromBody] UpdateSortsModel model)
	{
		return Ok(await Mediator.Send(new UpdateSetSorts.Command(Member.Id, model)));
	}
}
