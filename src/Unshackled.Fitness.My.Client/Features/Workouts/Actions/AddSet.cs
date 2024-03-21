using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Actions;

public class AddSet
{
	public class Command : IRequest<CommandResult<FormWorkoutSetModel>>
	{
		public FormWorkoutSetModel Model { get; private set; }

		public Command(FormWorkoutSetModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseWorkoutHandler, IRequestHandler<Command, CommandResult<FormWorkoutSetModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<FormWorkoutSetModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<FormWorkoutSetModel, FormWorkoutSetModel>($"{baseUrl}add-set", request.Model)
				?? new CommandResult<FormWorkoutSetModel>(false, Globals.UnexpectedError);
		}
	}
}
