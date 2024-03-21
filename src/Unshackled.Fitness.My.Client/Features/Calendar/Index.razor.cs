using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Web;
using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.Calendar.Actions;
using Unshackled.Fitness.My.Client.Features.Calendar.Models;

namespace Unshackled.Fitness.My.Client.Features.Calendar;

public class IndexBase : BaseComponent, IAsyncDisposable
{
	[CascadingParameter(Name = AppFrame.ParameterThemeProvider)]
	protected MudThemeProvider ThemeProvider { get; set; } = default!;

	[Inject] protected ILocalStorage localStorageService { get; set; } = default!;

	protected CalendarModel CalendarModel { get; set; } = new();
	protected SearchCalendarModel SearchModel { get; set; } = new();
	protected FormSearchModel FormModel { get; set; } = new();
	protected bool IsDrawerOpen { get; set; }
	protected bool IsLoading { get; set; } = true;
	protected bool IsSaving { get; set; } = false;
	protected bool DisableControls => IsSaving || IsLoading;
	protected string ThemeColor { get; set; } = string.Empty;
	protected Dictionary<string, bool> FilterVisibility { get; set; } = new();
	protected List<PresetModel> Presets { get; set; } = new();
	protected bool SystemIsDark { get; set; } = false;

	private string visibilityKey = "MetricVisibility";
	private string searchKey = "SearchMetrics";
	private DateTime defaultDate = new DateTime(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, 1);

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		Breadcrumbs.Add(new BreadcrumbItem("Calendar", null, true));

		await InitializeSearchForm();
		FilterVisibility = await localStorageService.GetItemAsync<Dictionary<string, bool>>(visibilityKey) ?? new();
				
		SetThemeColor();
		State.OnThemeChange += HandleThemeChanged;

		await GetCalendar();
		IsLoading = false;

		Presets = await Mediator.Send(new ListPresets.Query());
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && ThemeProvider is not null)
		{
			SystemIsDark = await ThemeProvider.GetSystemPreference();
			StateHasChanged();
		}
	}

	public override ValueTask DisposeAsync()
	{
		State.OnThemeChange -= HandleThemeChanged;
		return base.DisposeAsync();
	}

	private async Task GetCalendar()
	{
		SearchModel.FromDateUtc = SearchModel.FromDate.ToDateTime(new TimeOnly(0, 0, 0), DateTimeKind.Local).ToUniversalTime();
		SearchModel.ToDateUtc = SearchModel.ToDate.ToDateTime(new TimeOnly(0, 0, 0), DateTimeKind.Local).ToUniversalTime();
		SearchModel.WorkoutColor = ThemeColor;

		CalendarModel = await Mediator.Send(new GetCalendar.Query(SearchModel));

		foreach (var filter in CalendarModel.BlockFilters)
		{
			if (FilterVisibility.ContainsKey(filter.FilterId))
			{
				filter.IsChecked = FilterVisibility[filter.FilterId];
			}
			if(!filter.IsChecked)
			{
				SetBlockVisibility(filter);
			}
		}
	}

	protected string GetMonthDisplay(int num)
	{
		string output = "--";
		if (FormModel.EndDate.HasValue && num > 0)
		{
			string title = num == 1 ? "month" : "months";
			output = $"{num} {title} ({FormModel.EndDate.Value.AddMonths(-num).ToString("MM/yyyy")})";
		}
		return output;
	}

	protected async Task HandleFiltersChanged(List<CalendarBlockFilterModel> filters)
	{
		await SetFilterVisibility(filters);
	}

	protected async Task HandlePresetAdded(string title)
	{
		IsSaving = true;
		FormPresetModel model = new()
		{
			Settings = JsonSerializer.Serialize(FilterVisibility),
			Title = title
		};
		var result = await Mediator.Send(new AddPreset.Command(model));		
		if (result.Success && result.Payload != null)
		{
			Presets.Add(result.Payload);
			Presets = Presets.OrderBy(x => x.Title).ToList();
		}		
		ShowNotification(result);
		IsSaving = false;
	}

	protected async Task HandlePresetApplied(List<CalendarBlockFilterModel> filters)
	{
		await SetFilterVisibility(filters);
		IsDrawerOpen = false;
	}

	protected async Task HandlePresetDeleted(PresetModel model)
	{
		IsSaving = true;
		var result = await Mediator.Send(new DeletePreset.Command(model.Sid));
		if (result.Success)
		{
			Presets.Remove(model);
		}
		ShowNotification(result);
		IsSaving = false;
	}

	protected async Task HandlePresetUpdated(PresetModel model)
	{
		IsSaving = true;
		var result = await Mediator.Send(new UpdatePreset.Command(model.Sid, JsonSerializer.Serialize(FilterVisibility)));
		if (result.Success && result.Payload != null)
		{
			model.Settings = result.Payload.Settings;
		}
		ShowNotification(result);
		IsSaving = false;
	}

	protected async Task HandleResetClicked()
	{
		await InitializeSearchForm(true);
		await localStorageService.RemoveItemAsync(searchKey);
		await GetCalendar();
	}

	protected async Task HandleSearchClicked()
	{
		SearchModel.ToDate = FormModel.EndDate.HasValue ? DateOnly.FromDateTime(FormModel.EndDate.Value.AddMonths(1).AddDays(-1)) : DateOnly.FromDateTime(defaultDate);
		SearchModel.FromDate = SearchModel.ToDate.AddMonths(-(FormModel.NumberOfMonths + 1)).AddDays(1);
		await localStorageService.SetItemAsync(searchKey, FormModel, CancellationToken.None);
		await GetCalendar();
	}

	protected void HandleShowVisibilityClicked()
	{
		IsDrawerOpen = true;
	}

	protected void HandleThemeChanged()
	{
		SetThemeColor();
		foreach (var day in CalendarModel.Days)
		{
			foreach (var block in day.Blocks)
			{
				if (block.FilterId == "workout")
				{
					block.Color = ThemeColor;
				}
			}
		}
		StateHasChanged();
	}

	private async Task InitializeSearchForm(bool reset = false)
	{
		var defaultModel = new FormSearchModel
		{
			EndDate = new DateTime(DateTimeOffset.Now.Date.Year, DateTimeOffset.Now.Date.Month, 1),
			NumberOfMonths = 0
		};
		
		FormModel = reset ? defaultModel : await localStorageService.GetItemAsync<FormSearchModel>(searchKey) ?? defaultModel;

		SearchModel = new()
		{
			ToDate = FormModel.EndDate.HasValue ? DateOnly.FromDateTime(FormModel.EndDate.Value.AddMonths(1).AddDays(-1)) : DateOnly.FromDateTime(defaultDate)
		};
		SearchModel.FromDate = SearchModel.ToDate.AddMonths(-(FormModel.NumberOfMonths + 1)).AddDays(1);
	}

	private async Task SetFilterVisibility(List<CalendarBlockFilterModel> filters)
	{
		foreach (var filter in filters)
		{
			if (FilterVisibility.ContainsKey(filter.FilterId))
			{
				FilterVisibility[filter.FilterId] = filter.IsChecked;
			}
			else
			{
				FilterVisibility.Add(filter.FilterId, filter.IsChecked);
			}
			SetBlockVisibility(filter);
		}
		await localStorageService.SetItemAsync(visibilityKey, FilterVisibility, CancellationToken.None);
	}

	private void SetBlockVisibility(CalendarBlockFilterModel model)
	{
		foreach (var day in CalendarModel.Days)
		{
			foreach (var block in day.Blocks)
			{
				if (block.FilterId == model.FilterId)
				{
					block.IsVisible = model.IsChecked;
				}
			}
		}
	}

	private void SetThemeColor()
	{
		AppTheme theme = new();
		ThemeColor = State.Theme switch
		{
			Themes.Dark => theme.PaletteDark.Primary.Value,
			Themes.Light => theme.Palette.Primary.Value,
			Themes.System => SystemIsDark 
				? theme.PaletteDark.Primary.Value
				: theme.Palette.Primary.Value,
			_ => theme.Palette.Primary.Value
		};
	}
}