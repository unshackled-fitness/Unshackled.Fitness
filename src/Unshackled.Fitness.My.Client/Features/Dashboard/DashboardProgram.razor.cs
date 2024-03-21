using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.Dashboard.Actions;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;

namespace Unshackled.Fitness.My.Client.Features.Dashboard;

public class DashboardProgramBase : BaseComponent
{
	protected List<ProgramListModel> Templates { get; set; } = new();
	public bool IsWorking { get; set; }
	public bool IsSkipping { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		Templates = await Mediator.Send(new ListProgramItems.Query(DateTimeOffset.Now.Date));
	}

	protected async Task HandleSkipClicked(ProgramListModel model)
	{
		IsSkipping = true;

		var result = await Mediator.Send(new SkipWorkout.Command(model.ProgramSid));
		if (result.Success)
		{
			int idx = Templates.IndexOf(model);
			Templates.Remove(model);
			if (result.Payload != null)
			{
				Templates.Insert(idx, result.Payload);
			}
		}

		IsSkipping = false;
	}

	protected async Task HandleTrackClicked(string sid)
	{
		IsWorking = true;
		var result = await Mediator.Send(new AddWorkout.Command(sid));
		if (result.Success)
		{
			NavManager.NavigateTo($"/workouts/{result.Payload}");
		}
		else
		{
			ShowNotification(result);
		}
		IsWorking = false;
	}
}