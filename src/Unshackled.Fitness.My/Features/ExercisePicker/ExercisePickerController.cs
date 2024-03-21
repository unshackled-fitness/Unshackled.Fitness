using Microsoft.AspNetCore.Mvc;
using Unshackled.Fitness.My.Client.Features.ExercisePicker.Models;
using Unshackled.Fitness.My.Features.ExercisePicker.Actions;

namespace Unshackled.Fitness.My.Features.ExercisePicker;

[ApiController]
[ApiRoute("exercise-picker")]
public class ExercisePickerController : BaseController
{
	[HttpPost("search")]
	public async Task<IActionResult> Search([FromBody] SearchExerciseModel model)
	{
		return Ok(await Mediator.Send(new SearchExercises.Query(Member.Id, model)));
	}
}
