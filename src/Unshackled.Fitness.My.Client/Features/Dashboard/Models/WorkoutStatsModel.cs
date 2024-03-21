using System.Text.Json.Serialization;

namespace Unshackled.Fitness.My.Client.Features.Dashboard.Models;

public class WorkoutStatsModel
{
	public DateTime ToDateUtc { get; set; } = DateTime.UtcNow;
	public List<WorkoutModel> Workouts { get; set; } = new();
	public List<int> Years { get; set; } = new();
	public int TotalWorkouts { get; set; }
	public decimal TotalVolumeLb { get; set; }
	public decimal TotalVolumeKg { get; set; }

	[JsonIgnore]
	public WeekModel[] Weeks { get; set; } = new WeekModel[53];

	public void Fill()
	{
		int workoutIdx = 0;
		DateTime toDate = ToDateUtc.ToLocalTime().Date;
		DateTime currentDate = ToDateUtc.ToLocalTime().Date.AddYears(-1);
		int firstDayOfWeek = (int)currentDate.DayOfWeek;
		for (int w = 0; w < Weeks.Length; w++)
		{
			Weeks[w] = new()
			{
				Month = currentDate.ToString("MMM")
			};

			for (int d = 0; d < 7; d++)
			{
				Weeks[w].Days[d] = new();
				if (d >= firstDayOfWeek && currentDate < toDate)
				{
					Weeks[w].Days[d].Date = currentDate;
					while (workoutIdx < Workouts.Count && Workouts[workoutIdx].DateCompletedUtc.ToLocalTime().Date == currentDate)
					{
						Weeks[w].Days[d].WorkoutCount++;
						if (Weeks[w].Days[d].WorkoutCount == 1)
							Weeks[w].Days[d].WorkoutTitle = Workouts[workoutIdx].Title;
						else
							Weeks[w].Days[d].WorkoutTitle = string.Empty;

						workoutIdx++;
					}

					currentDate = currentDate.AddDays(1);
				}
			}
			firstDayOfWeek = 0;
		}
	}

	public int GetWeekColumnsInMonth(int startIdx)
	{
		string month = Weeks[startIdx].Month;
		int count = 1;
		for (int i = startIdx + 1; i < Weeks.Length; i++)
		{
			if (Weeks[i].Month == month)
				count++;
			else
				break;
		}
		return count;
	}

	public class WeekModel
	{
		public string Month { get; set; } = string.Empty;
		public DayModel[] Days { get; set; } = new DayModel[7];
	}

	public class DayModel
	{
		public DateTime? Date { get; set; }
		public int WorkoutCount { get; set; }
		public string WorkoutTitle { get; set; } = string.Empty;

		public string Description
		{
			get
			{
				if (Date == null)
					return string.Empty;

				if (WorkoutCount == 0)
					return $"No workouts on {Date.Value.ToString("D")}";

				return WorkoutCount > 1
					? $"{WorkoutCount} workouts on {Date.Value.ToString("D")}"
					: $"{WorkoutTitle} on {Date.Value.ToString("D")}";
			}
		}
	}
}
