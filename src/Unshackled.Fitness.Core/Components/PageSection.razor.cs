using Microsoft.AspNetCore.Components;

namespace Unshackled.Fitness.Core.Components;

public class PageSectionBase : BaseComponent
{
	[Parameter] public bool IsEditMode { get; set; } = false;
	[Parameter] public bool DisableSectionControls { get; set; } = false;
	[Parameter] public bool IsLoading { get; set; } = false;
	[Parameter] public bool IsWorking { get; set; } = false;
	[Parameter] public string Title { get; set; } = "Section";
	[Parameter] public EventCallback<bool> IsEditingSectionChanged { get; set; }
	[Parameter] public EventCallback EditClicked { get; set; }
	[Parameter] public EventCallback UpdateClicked { get; set; }

	[Parameter] public RenderFragment DisplayContent { get; set; } = default!;
	[Parameter] public RenderFragment EditContent { get; set; } = default!;

	protected bool Editing { get; set; } = false;

	protected bool DisableControls => IsLoading || IsWorking;

	protected async Task<bool> UpdateIsEditingSection(bool value)
	{
		if (IsEditingSectionChanged.HasDelegate)
			await IsEditingSectionChanged.InvokeAsync(value);
		return value;
	}

	protected async Task HandleCancelEditClicked()
	{
		Editing = await UpdateIsEditingSection(false);
	}

	protected async Task HandleEditClicked()
	{
		if (EditClicked.HasDelegate)
			await EditClicked.InvokeAsync();
		Editing = await UpdateIsEditingSection(true);
	}

	protected async Task HandleUpdateClicked()
	{
		if(UpdateClicked.HasDelegate)
			await UpdateClicked.InvokeAsync();
		Editing = await UpdateIsEditingSection(false);
	}
}