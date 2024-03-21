using MediatR;
using Unshackled.Fitness.My.Client.Features.Calendar.Models;

namespace Unshackled.Fitness.My.Client.Features.Calendar.Actions;

public class ListPresets
{
	public class Query : IRequest<List<PresetModel>> 
	{
		
	}

	public class Handler : BaseCalendarHandler, IRequestHandler<Query, List<PresetModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<List<PresetModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<List<PresetModel>>($"{baseUrl}list-presets") ?? new();
		}
	}
}
