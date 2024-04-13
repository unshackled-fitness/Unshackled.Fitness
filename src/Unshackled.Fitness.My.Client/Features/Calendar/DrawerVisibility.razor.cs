using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.Core.Models.Calendars;
using Unshackled.Fitness.My.Client.Features.Calendar.Models;

namespace Unshackled.Fitness.My.Client.Features.Calendar;

public class DrawerStatsBase : BaseComponent
{
	[Inject] protected IDialogService DialogService { get; set; } = default!;
	[Parameter] public List<CalendarBlockFilterGroupModel> FilterGroups { get; set; } = new();
	[Parameter] public List<CalendarBlockFilterModel> Filters { get; set; } = new();
	[Parameter] public EventCallback<List<CalendarBlockFilterModel>> FiltersChanged { get; set; }
	[Parameter]	public List<PresetModel> Presets { get; set; } = new();
	[Parameter] public EventCallback<string> PresetAdded { get; set; }
	[Parameter] public EventCallback<List<CalendarBlockFilterModel>> PresetApplied { get; set; }
	[Parameter] public EventCallback<PresetModel> PresetDeleted { get; set; }
	[Parameter] public EventCallback<PresetModel> PresetUpdated { get; set; }

	protected bool IsSaving { get; set; }

	protected bool? GetAllCheckboxState()
	{
		if (Filters.Where(x => x.IsChecked == true).Count() == Filters.Count())
			return true;
		else if (Filters.Where(x => x.IsChecked == false).Count() == Filters.Count())
			return false;
		else
			return null;
	}

	protected string GetItemStyle(CalendarBlockFilterModel filter)
	{
		if (!string.IsNullOrEmpty(filter.Color))
		{
			return $"border-left: .5rem solid {filter.Color}";
		}
		return string.Empty;
	}

	protected async Task HandleApplyPresetClicked(PresetModel preset)
	{
		var filters = JsonSerializer.Deserialize<Dictionary<string, bool>>(preset.Settings)
			?? new Dictionary<string, bool>();

		foreach (var blockFilter in Filters)
		{
			if (filters.ContainsKey(blockFilter.FilterId))
			{
				blockFilter.IsChecked = filters[blockFilter.FilterId];
			}
		}
		await PresetApplied.InvokeAsync(Filters);
		StateHasChanged();
	}

	protected bool? GetGroupCheckboxState(CalendarBlockFilterGroupModel group)
	{
		var filters = Filters.Where(x => x.ListGroupSid == group.Sid).ToArray();
		if (filters.Where(x => x.IsChecked == true).Count() == filters.Count())
			return true;
		else if (filters.Where(x => x.IsChecked == false).Count() == filters.Count())
			return false;
		else
			return null;
	}

	protected async Task HandleAddPresetClicked()
	{
		var options = new DialogOptions { ClassBackground = "bg-blur", MaxWidth = MaxWidth.Medium };

		var dialog = DialogService.Show<DialogSavePreset>("Save As Preset", options);
		var result = await dialog.Result;
		if (!result.Canceled)
		{
			IsSaving = true;
			await PresetAdded.InvokeAsync(result.Data.ToString());
			IsSaving = false;
		}
	}

	protected async Task HandleCheckAllClicked(bool? isChecked)
	{
		if (isChecked.HasValue)
		{
			foreach (var filter in Filters)
			{
				filter.IsChecked = isChecked.Value;
			}
			await FiltersChanged.InvokeAsync(Filters);
		}
		StateHasChanged();
	}

	protected async Task HandleCheckAllInGroupClicked(CalendarBlockFilterGroupModel filterGroup, bool? isChecked)
	{
		filterGroup.AllCheckedState = isChecked;
		if (isChecked.HasValue)
		{
			foreach (var filter in Filters.Where(x => x.ListGroupSid == filterGroup.Sid).ToArray())
			{
				filter.IsChecked = isChecked.Value;
			}
			await FiltersChanged.InvokeAsync(Filters);
		}
		StateHasChanged();
	}

	protected async Task HandleDefinitionChecked(CalendarBlockFilterModel item, bool value)
	{
		if (item.IsChecked != value)
		{
			item.IsChecked = value;
			await FiltersChanged.InvokeAsync(Filters);
			StateHasChanged();
		}
	}

	protected async Task HandleDeletePresetClicked(PresetModel preset)
	{
		bool? confirm = await DialogService.ShowMessageBox(
				"Confirm",
				"Are you sure you want to delete this preset?",
				yesText: "Delete", cancelText: "Cancel");

		if (confirm.HasValue && confirm.Value)
		{
			IsSaving = true;
			await PresetDeleted.InvokeAsync(preset);
			IsSaving = false;
		}
	}

	protected async Task HandleUpdatePresetClicked(PresetModel preset)
	{
		IsSaving = true;
		await PresetUpdated.InvokeAsync(preset);
		IsSaving = false;
	}
}