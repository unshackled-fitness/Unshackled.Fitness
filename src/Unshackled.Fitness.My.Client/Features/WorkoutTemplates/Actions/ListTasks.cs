using MediatR;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;

public class ListTasks
{
	public class Query : IRequest<List<TemplateTaskModel>>
	{
		public string Sid { get; private set; }
		public WorkoutTaskTypes TaskType { get; private set; }

		public Query(string sid, WorkoutTaskTypes taskType)
		{
			Sid = sid;
			TaskType = taskType;
		}
	}

	public class Handler : BaseTemplateHandler, IRequestHandler<Query, List<TemplateTaskModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<List<TemplateTaskModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<List<TemplateTaskModel>>($"{baseUrl}get/{request.Sid}/tasks/{(int)request.TaskType}") ??
				new List<TemplateTaskModel>();
		}
	}
}
