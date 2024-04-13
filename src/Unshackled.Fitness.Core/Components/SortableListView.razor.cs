using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Extensions;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.Core.Components;

public partial class SortableListView<TItem> where TItem : ISortable, ICloneable, new()
{
	[Parameter] public bool AllowSort { get; set; } = true;
	[Parameter] public string? Class { get; set; }
	[Parameter] public bool DisableSort { get; set; } = false;
	[Parameter] public RenderFragment<TItem>? DraggableItemRenderer { get; set; }
	[Parameter] public string? DropZoneClass { get; set; }
	[Parameter] public RenderFragment? EmptyListRenderer { get; set; }
	[Parameter] public bool IsLoading { get; set; } = false;
	[Parameter] public EventCallback<bool> IsSortingChanged { get; set; }
	[Parameter] public string? ItemDraggingClass { get; set; }
	[Parameter] public RenderFragment<RowContext<TItem>>? ItemRenderer { get; set; }
	[Parameter] public List<TItem> Items { get; set; } = new();
	[Parameter] public RenderFragment? ListTools { get; set; }
	[Parameter] public EventCallback<List<TItem>> SortOrderChanged { get; set; }
	[Parameter] public HorizontalAlignment ToolbarAlignment { get; set; } = HorizontalAlignment.Start;
	[Parameter] public ToolbarPositions ToolbarPosition { get; set; } = ToolbarPositions.Top;
	[Parameter] public int TotalItems { get; set; }
	[Parameter] public bool UseDividers { get; set; } = true;

	protected bool IsReordering { get; set; } = false;
	protected string ListClass => new CssBuilder("list-view")
		.AddClass(Class)
		.Build();
	protected List<TItem> SortingItems { get; set; } = new();
	protected bool IsWorking { get; set; } = false;
	protected string ZoneClass =>
		new CssBuilder("mud-paper")
			.AddClass($"mud-paper-outlined")
			.AddClass($"no-background")
			.AddClass($"mud-elevation-0")
			.AddClass($"py-2 px-4 mb-4")
			.AddClass(DropZoneClass)
		.Build();

	protected async Task HandleCancelSortClicked()
	{
		IsReordering = false;
		await IsSortingChanged.InvokeAsync(false);
	}

	protected void HandleItemDropped(MudItemDropInfo<TItem> dropItem)
	{
		if (dropItem.Item != null)
		{
			int newIdx = dropItem.IndexInZone;
			int oldIdx = SortingItems.IndexOf(dropItem.Item);
			SortingItems.MoveAndSort(oldIdx, newIdx);
		}
	}

	protected async Task HandleSortClicked()
	{
		SortingItems = Items.ConvertAll(x => (TItem)x.Clone());
		IsReordering = true;
		await IsSortingChanged.InvokeAsync(true);
	}

	protected async Task HandleUpdateSortClicked()
	{
		IsWorking = true;
		await SortOrderChanged.InvokeAsync(SortingItems);
		await IsSortingChanged.InvokeAsync(false);
		IsReordering = false;
		IsWorking = false;
	}
}