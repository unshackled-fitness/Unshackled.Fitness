namespace Unshackled.Fitness.My.Client.Features.Exercises.Actions;

public abstract class BaseExerciseHandler : BaseHandler
{
	public BaseExerciseHandler(HttpClient httpClient) : base(httpClient, "exercises") { }
}
