namespace Unshackled.Fitness.My.Client.Features.ExercisePicker.Actions;

public abstract class BaseExercisePickerHandler : BaseHandler
{
	public BaseExercisePickerHandler(HttpClient httpClient) : base(httpClient, "exercise-picker") { }
}
