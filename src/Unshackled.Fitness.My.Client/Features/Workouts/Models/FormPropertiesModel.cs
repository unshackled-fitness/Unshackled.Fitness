using System.Text.Json.Serialization;
using FluentValidation;
using Unshackled.Fitness.Core.Extensions;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Models;

public class FormPropertiesModel
{
	public string Sid { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public bool IsStarted { get; set; }
	public bool IsComplete { get; set; }
	public DateTime? DateStartedUtc { get; set; }
	public DateTime? DateCompletedUtc { get; set; }
	public int Rating { get; set; }
	public string? Notes { get; set; }

	private DateTime? dateStarted;
	[JsonIgnore]
	public DateTime? DateStarted
	{
		get
		{
			if(!dateStarted.HasValue)
				dateStarted = DateStartedUtc.HasValue ? DateStartedUtc.Value.ToLocalTime().Date : null;
			return dateStarted;
		}
		set
		{
			dateStarted = value;
			DateStartedUtc = dateStarted.CombineDateAndTime(TimeStarted);
			if (DateStartedUtc.HasValue)
			{
				DateStartedUtc = DateStartedUtc.Value.ToUniversalTime();
			}
		}
	}

	private TimeSpan? timeStarted;
	[JsonIgnore]
	public TimeSpan? TimeStarted
	{
		get
		{
			if(!timeStarted.HasValue)
				timeStarted = DateStartedUtc.HasValue ? DateStartedUtc.Value.ToLocalTime().TimeOfDay : null;
			return timeStarted;
		}
		set
		{
			timeStarted = value;
			DateStartedUtc = timeStarted.CombineDateAndTime(DateStarted);
			if (DateStartedUtc.HasValue)
			{
				DateStartedUtc = DateStartedUtc.Value.ToUniversalTime();
			}
		}
	}

	private DateTime? dateCompleted;
	[JsonIgnore]
	public DateTime? DateCompleted
	{
		get
		{
			if (!dateCompleted.HasValue)
				dateCompleted = DateCompletedUtc.HasValue ? DateCompletedUtc.Value.ToLocalTime().Date : null;
			return dateCompleted;
		}
		set
		{
			dateCompleted = value;
			DateCompletedUtc = dateCompleted.CombineDateAndTime(TimeCompleted);
			if (DateCompletedUtc.HasValue)
			{
				DateCompletedUtc = DateCompletedUtc.Value.ToUniversalTime();
			}
		}
	}

	private TimeSpan? timeCompleted;
	[JsonIgnore]
	public TimeSpan? TimeCompleted
	{
		get
		{
			if (!timeCompleted.HasValue)
				timeCompleted = DateCompletedUtc.HasValue ? DateCompletedUtc.Value.ToLocalTime().TimeOfDay : null;
			return timeCompleted;
		}
		set
		{
			timeCompleted = value;
			DateCompletedUtc = timeCompleted.CombineDateAndTime(DateCompleted);
			if (DateCompletedUtc.HasValue)
			{
				DateCompletedUtc = DateCompletedUtc.Value.ToUniversalTime();
			}
		}
	}

	public string DateFormat(DateTime? date)
	{
		if (date.HasValue)
			return "MM/dd/yyyy";
		else
			return string.Empty;
	}

	public class Validator : BaseValidator<FormPropertiesModel>
	{
		public Validator()
		{
			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Title is required.")
				.MaximumLength(255).WithMessage("Title must not exceed 255 characters.");

			When(x => x.IsStarted, () =>
			{
				RuleFor(x => x.DateStartedUtc)
					.NotNull().WithMessage("A start date is required.");
			});

			When(x => x.IsComplete, () =>
			{
				RuleFor(x => x.DateCompletedUtc)
					.NotNull().WithMessage("A completion date is required.")
					.GreaterThanOrEqualTo(x => x.DateStartedUtc).WithMessage("Completion date must be after the Start Date.");
			});

		}
	}
}
