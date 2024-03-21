using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Utils;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Dashboard.Actions;

public class ListProgramItems
{
	public class Query : IRequest<List<ProgramListModel>>
	{
		public long MemberId { get; private set; }
		public DateTime DisplayDate { get; private set; }

		public Query(long memberId, DateTime displayDate)
		{
			MemberId = memberId;
			DisplayDate = displayDate;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, List<ProgramListModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<List<ProgramListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			var programs = await db.Programs
				.AsNoTracking()
				.Where(x => x.MemberId == request.MemberId 
					&& x.DateStarted != null
					&& x.DateStarted <= request.DisplayDate.Date.SetKindUtc())
				.ToListAsync();

			var list = new List<ProgramListModel>();
			List<long> matchingIds = new();

			foreach (var program in programs)
			{
				if (program.ProgramType == ProgramTypes.Sequential)
				{
					// Skip if we are on the same day as the last workout.
					if (program.DateLastWorkoutUtc >= request.DisplayDate.Date.ToUniversalTime()
						&& program.DateLastWorkoutUtc < request.DisplayDate.Date.AddDays(1).ToUniversalTime())
						break;

					ProgramTemplateEntity? template = null;

					// SortOrder == NextTemplateIndex may not exist if a template was deleted
					// so get next SortOrder >= NextTemplateIndex to account for missing numbers in the sequence
					template = await db.ProgramTemplates
						.AsNoTracking()
						.Include(x => x.WorkoutTemplate)
						.Where(x => x.ProgramId == program.Id && x.SortOrder >= program.NextTemplateIndex)
						.OrderBy(x => x.SortOrder)
						.FirstOrDefaultAsync();

					// Nothing found if we were at the end of the list
					if (template == null)
					{
						// Get first
						template = await db.ProgramTemplates
							.AsNoTracking()
							.Include(x => x.WorkoutTemplate)
							.Where(x => x.ProgramId == program.Id)
							.OrderBy(x => x.SortOrder)
							.FirstOrDefaultAsync();
					}

					// Add if found and it doesn't already exist in list
					if (template != null 
						&& !list.Where(x => x.Sid == template.WorkoutTemplateId.Encode()).Any())
					{
						list.Add(new()
						{
							IsStarted = false,
							ProgramSid = program.Id.Encode(),
							ProgramTitle = program.Title,
							ProgramType = ProgramTypes.Sequential,
							Sid = template.WorkoutTemplateId.Encode(),
							Title = template.WorkoutTemplate.Title
						});
						matchingIds.Add(template.WorkoutTemplateId);
					}
				}
				else // Fixed Repeating
				{
					Calculator.WeekAndDayInCycle(program.DateStarted!.Value, request.DisplayDate.Date, program.LengthWeeks, 
						out int week, out int day);

					var templates = await db.ProgramTemplates
						.AsNoTracking()
						.Include (x => x.WorkoutTemplate)
						.Where(x => x.ProgramId == program.Id
							&& x.WeekNumber == week
							&& x.DayNumber == day)
						.OrderBy(x => x.SortOrder)
						.ToListAsync();

					foreach ( var template in templates )
					{
						// Add if it doesn't already exist in list
						if (!list.Where(x => x.Sid == template.WorkoutTemplateId.Encode()).Any())
						{
							list.Add(new()
							{
								IsStarted = false,
								ProgramSid = program.Id.Encode(),
								ProgramTitle = program.Title,
								ProgramType = ProgramTypes.FixedRepeating,
								Sid = template.WorkoutTemplateId.Encode(),
								Title = template.WorkoutTemplate.Title
							});
							matchingIds.Add(template.WorkoutTemplateId);
						}
					}
				}
			}

			if (list.Any())
			{
				// Get workouts from DisplayDate with matching template IDs
				var workouts = await db.Workouts
					.AsNoTracking()
					.Where(x => x.DateCreatedUtc >= request.DisplayDate.Date.ToUniversalTime()
						&& x.DateCreatedUtc < request.DisplayDate.Date.AddDays(1).ToUniversalTime()
						&& x.WorkoutTemplateId.HasValue
						&& matchingIds.Contains(x.WorkoutTemplateId.Value))
					.ToListAsync();

				foreach (var workout in workouts)
				{
					// Get items with matching SID.
					var item = list
						.Where(x => x.Sid == workout.WorkoutTemplateId!.Value.Encode())
						.FirstOrDefault();

					// Change the item to a workout
					if (item != null)
					{
						item.IsStarted = true;
						item.IsCompleted = workout.DateCompletedUtc.HasValue;
						item.Sid = workout.Id.Encode();
						item.Title = workout.Title;
					}
				}
			}
			
			return list;
		}
	}
}
