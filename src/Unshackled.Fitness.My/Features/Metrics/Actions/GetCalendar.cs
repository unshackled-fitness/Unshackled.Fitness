using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models.Calendars;

namespace Unshackled.Fitness.My.Features.Metrics.Actions;

public class GetCalendar
{
	public class Query : IRequest<CalendarModel>
	{
		public long MemberId { get; private set; }
		public long Id { get; private set; }
		public DateOnly ToDate { get; private set; }

		public Query(long memberId, long id, DateOnly toDate)
		{
			MemberId = memberId;
			Id = id;
			ToDate = toDate;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, CalendarModel>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CalendarModel> Handle(Query request, CancellationToken cancellationToken)
		{
			DateOnly toDate = request.ToDate;
			DateOnly fromDate = toDate.AddYears(-1).AddDays(1);

			CalendarModel model = new()
			{
				ToDate = toDate,
				FromDate = fromDate
			};

			model.YearsAvailable = await db.Metrics
				.Where(x => x.MetricDefinitionId == request.Id 
					&& x.MemberId == request.MemberId)
				.OrderBy(x => x.DateRecorded)
				.Select(x => x.DateRecorded.Year)
				.Distinct()
				.ToListAsync();

			var blocks = await db.Metrics
				.AsNoTracking()
				.Include(x => x.MetricDefinition)
				.Where(x => x.MetricDefinitionId == request.Id 
					&& x.MemberId == request.MemberId
					&& x.DateRecorded > fromDate.ToDateTime(new TimeOnly(0, 0, 0), DateTimeKind.Local)
					&& x.DateRecorded <= toDate.ToDateTime(new TimeOnly(0, 0, 0), DateTimeKind.Local))
				.OrderBy(x => x.DateRecorded)
					.ThenBy(x => x.MetricDefinition.SortOrder)
				.Select(x => new
				{
					x.DateRecorded,
					x.MetricDefinition.MetricType,
					x.MetricDefinition.MaxValue,
					x.RecordedValue,
					Color = x.MetricDefinition.HighlightColor,
				})
				.ToListAsync();

			DateOnly currentDate = model.FromDate;
			int blockIdx = 0;
			while (currentDate <= model.ToDate)
			{
				CalendarDayModel day = new()
				{
					Date = currentDate
				};

				// add blocks for the current date
				while (blockIdx < blocks.Count && DateOnly.FromDateTime(blocks[blockIdx].DateRecorded) == currentDate)
				{
					CalendarBlockModel block = new()
					{
						Color = blocks[blockIdx].Color,
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

			return model;
		}
	}
}