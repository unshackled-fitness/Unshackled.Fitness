using MudBlazor;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.My.Client.Features.Programs.Actions;
using Unshackled.Fitness.My.Client.Features.Programs.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs;

public class IndexBase : BaseSearchComponent<SearchProgramModel, ProgramListModel>
{
	protected bool IsAdding { get; set; }
	protected FormAddProgramModel FormAddModel { get; set; } = new();
	protected FormAddProgramModel.Validator FormValidator { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		SearchKey = "SearchPrograms";

		Breadcrumbs.Add(new BreadcrumbItem("Programs", null, true));

		SearchModel = await GetLocalSetting(SearchKey) ?? new();

		await DoSearch(SearchModel.Page);
	}

	protected async override Task DoSearch(int page)
	{
		SearchModel.Page = page;

		IsLoading = true;
		await SaveLocalSetting(SearchKey, SearchModel);
		SearchResults = await Mediator.Send(new SearchPrograms.Query(SearchModel));
		IsLoading = false;
	}

	protected void HandleAddClicked()
	{
		FormAddModel = new()
		{
			ProgramType = ProgramTypes.FixedRepeating
		};
		IsAdding = true;
	}

	protected void HandleCancelAddClicked()
	{
		IsAdding = false;
	}

	protected async Task HandleFormSubmitted()
	{
		IsWorking = true;
		var result = await Mediator.Send(new AddProgram.Command(FormAddModel));
		IsAdding = false;
		IsWorking = false;
		ShowNotification(result);
		if (result.Success)
		{
			NavManager.NavigateTo($"/programs/{result.Payload}");
		}
	}
}