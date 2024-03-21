using MediatR;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Actions;

public class SelectLibraryExercises
{
	public class Query : IRequest<List<LibraryListModel>>
	{
		public List<Guid> Sids { get; private set; }

		public Query(List<Guid> sids)
		{
			Sids = sids;
		}
	}

	public class Handler : BaseApiHandler, IRequestHandler<Query, List<LibraryListModel>>
	{
		public Handler(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "libraries") { }

		public async Task<List<LibraryListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			if (request.Sids.Count > AppGlobals.MaxSelectionLimit)
				throw new Exception("Maximum selection size exceeded.");

			return await PostToResultAsync<List<Guid>, List<LibraryListModel>>($"{baseUrl}select-exercises", request.Sids) ?? new();
		}
	}
}