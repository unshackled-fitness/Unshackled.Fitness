using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models.Calendars;
using Unshackled.Fitness.My.Client.Features.Calendar.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Calendar.Actions;

public class GetCalendar
{
	public class Query : IRequest<CalendarModel>
	{
		public long MemberId { get; private set; }
		public SearchCalendarModel Model { get; private set; }

		public Query(long memberId, SearchCalendarModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, CalendarModel>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CalendarModel> Handle(Query request, CancellationToken cancellationToken)
		{
			DateOnly fromDate = request.Model.FromDate;
			DateOnly toDate = request.Model.ToDate;

			CalendarModel model = new()
			{
				FromDate = fromDate,
				ToDate = toDate
			};

			int oldestYear = await db.Metrics
				.Where(x => x.MemberId == request.MemberId)
				.OrderBy(x => x.DateRecorded)
				.Select(x => x.DateRecorded.Year)
				.FirstOrDefaultAsync();

			for (int i = oldestYear; i <= DateTimeOffset.Now.Year; i++)
			{
				model.YearsAvailable.Add(i);
			}

			var workouts = await db.Workouts
				.Where(x => x.MemberId == request.MemberId
					&& x.DateCompletedUtc != null
					&& x.DateCompletedUtc >= request.Model.FromDateUtc
					&& x.DateCompletedUtc <= request.Model.ToDateUtc)
				.OrderBy(x => x.DateCompletedUtc)
				.Select(x => new
				{
					x.DateCompletedUtc,
					x.Title
				})
				.ToListAsync();

			var blocks = await db.Metrics
				.AsNoTracking()
				.Include(x => x.MetricDefinition)
				.Where(x => x.MemberId == request.MemberId
					&& x.MetricDefinition.IsArchived == false
					&& x.DateRecorded >= fromDate.ToDateTime(new TimeOnly(0, 0, 0), DateTimeKind.Utc)
					&& x.DateRecorded <= toDate.ToDateTime(new TimeOnly(0, 0, 0), DateTimeKind.Utc))
				.OrderBy(x => x.DateRecorded)
					.ThenBy(x => x.MetricDefinition.SortOrder)
				.Select(x => new
				{
					x.MetricDefinitionId,
					x.DateRecorded,
					x.MetricDefinition.MetricType,
					x.MetricDefinition.MaxValue,
					x.RecordedValue,
					Color = x.MetricDefinition.HighlightColor,
				})
				.ToListAsync();


			// Fill Blocks
			DateOnly currentDate = model.FromDate;
			DateTime currentDateUtc = request.Model.FromDateUtc;
			int workoutIdx = 0;
			int blockIdx = 0;
			while (currentDate <= model.ToDate)
			{
				CalendarDayModel day = new()
				{
					Date = currentDate
				};

				// add workouts for the current date
				while (workoutIdx < workouts.Count
					&& workouts[workoutIdx].DateCompletedUtc!.Value >= currentDateUtc
					&& workouts[workoutIdx].DateCompletedUtc!.Value < currentDateUtc.AddDays(1))
				{
					CalendarBlockModel workout = new()
					{
						FilterId = "workout",
						Title = workouts[workoutIdx].Title,
						Color = request.Model.WorkoutColor,
						IsCentered = false
					};

					day.Blocks.Add(workout);
					workoutIdx++;
				}
				currentDateUtc = currentDateUtc.AddDays(1);

				// add blocks for the current date
				while (blockIdx < blocks.Count && DateOnly.FromDateTime(blocks[blockIdx].DateRecorded) == currentDate)
				{
					CalendarBlockModel block = new()
					{
						Color = blocks[blockIdx].Color,
						FilterId = blocks[blockIdx].MetricDefinitionId.Encode(),
						IsCentered = true
					};

					switch (blocks[blockIdx].MetricType)
					{
						case MetricTypes.ExactValue:
							block.Title = blocks[blockIdx].RecordedValue.ToString("#");
							break;
						case MetricTypes.Toggle:
							break;
						case MetricTypes.Counter:
							block.Title = blocks[blockIdx].RecordedValue.ToString("#");
							break;
						case MetricTypes.Range:
							block.Title = $"{blocks[blockIdx].RecordedValue.ToString("#")}/{blocks[blockIdx].MaxValue.ToString("#")}";
							break;
						default:
							break;
					}

					day.Blocks.Add(block);
					blockIdx++;
				}

				model.Days.Add(day);
				currentDate = currentDate.AddDays(1);
			}

			// Fill block definition groups
			model.BlockFilterGroups.Add(new CalendarBlockFilterGroupModel
			{
				Sid = "default",
				SortOrder = -1,
				Title = "Default"
			});

			var defGroups = await db.MetricDefinitionGroups
				.AsNoTracking()
				.Where(x => x.MemberId == request.MemberId)
				.OrderBy(x => x.SortOrder)
				.Select(x => new CalendarBlockFilterGroupModel
				{
					Sid = x.Id.Encode(),
					SortOrder = x.SortOrder,
					Title = x.Title
				})
				.ToListAsync();

			model.BlockFilterGroups.AddRange(defGroups);

			// Fill block definitions			
			model.BlockFilters.Add(new CalendarBlockFilterModel
			{
				Color = request.Model.WorkoutColor,
				FilterId = "workout",
				ListGroupSid = "default",
				Title = "Workouts"
			});

			var definitions = await db.MetricDefinitions
				.AsNoTracking()
				.Where(x => x.MemberId == request.MemberId && x.IsArchived == false)
				.OrderBy(x => x.SortOrder)
				.Select(x => new CalendarBlockFilterModel
				{
					Color = x.HighlightColor,
					FilterId = x.Id.Encode(),
					ListGroupSid = x.ListGroupId.Encode(),
					SortOrder = x.SortOrder,
					SubTitle = x.SubTitle,
					Title = x.Title
				})
				.ToListAsync();

			model.BlockFilters.AddRange(definitions);

			return model;
		}
	}
}