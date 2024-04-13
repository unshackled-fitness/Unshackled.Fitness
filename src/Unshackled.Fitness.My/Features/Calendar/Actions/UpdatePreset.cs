using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Calendar.Models;

namespace Unshackled.Fitness.My.Features.Calendar.Actions;

public class UpdatePreset
{
	public class Command : IRequest<CommandResult<PresetModel>>
	{
		public long MemberId { get; private set; }
		public long PresetId { get; private set; }
		public string Settings { get; private set; }

		public Command(long memberId, long presetId, string settings)
		{
			MemberId = memberId;
			PresetId = presetId;
			Settings = settings;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<PresetModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<PresetModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			var preset = await db.MetricPresets
				.Where(x => x.MemberId == request.MemberId && x.Id == request.PresetId)
				.SingleOrDefaultAsync();

			if (preset == null) 
				return new CommandResult<PresetModel>(false, "Invalid preset.");

			preset.Settings = request.Settings;
			await db.SaveChangesAsync();

			return new CommandResult<PresetModel>(true, "Preset updated.", mapper.Map<PresetModel>(preset));
		}
	}
}