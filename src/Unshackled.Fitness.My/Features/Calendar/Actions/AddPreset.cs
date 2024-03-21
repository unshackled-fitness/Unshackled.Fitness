using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.My.Client.Features.Calendar.Models;

namespace Unshackled.Fitness.My.Features.Calendar.Actions;

public class AddPreset
{
	public class Command : IRequest<CommandResult<PresetModel>>
	{
		public long MemberId { get; private set; }
		public FormPresetModel Model { get; private set; }

		public Command(long memberId, FormPresetModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<PresetModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<PresetModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			bool exists = await db.MetricPresets
				.Where(x => x.MemberId == request.MemberId && x.Title == request.Model.Title)
				.AnyAsync();

			if (exists) 
				return new CommandResult<PresetModel>(false, "A preset with that name already exists.");

			MetricPresetEntity preset = new()
			{
				MemberId = request.MemberId,
				Settings = request.Model.Settings,
				Title = request.Model.Title
			};
			db.MetricPresets.Add(preset);
			await db.SaveChangesAsync();

			return new CommandResult<PresetModel>(true, "Preset saved.", mapper.Map<PresetModel>(preset));
		}
	}
}