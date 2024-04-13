using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using Unshackled.Fitness.Core.Extensions;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.Core.Components;

public partial class SortableForm<TItem> where TItem : ISortable, new()
{
	[Parameter] public List<TItem> Items { get; set; } = new();
	[Parameter] public RenderFragment<TItem>? ItemRenderer { get; set; }
	[Parameter] public string? DropZoneClass { get; set; }
	[Parameter] public string? ItemDraggingClass { get; set; }

	[Parameter] public EventCallback<List<TItem>> ItemsChanged { get; set; }
	[Parameter]	public EventCallback<MudItemDropInfo<TItem>> ItemDropped { get; set; }

	protected string ZoneClass =>
		new CssBuilder("mud-paper")
			.AddClass($"mud-paper-outlined")
			.AddClass($"no-background")
			.AddClass($"mud-elevation-0")
			.AddClass($"py-2 px-4 mb-4")
			.AddClass(DropZoneClass)
		.Build();

	protected async Task HandleItemDropped(MudItemDropInfo<TItem> dropItem)
	{
		if (dropItem.Item != null)
		{
			int newIdx = dropItem.IndexInZone;
			int oldIdx = Items.IndexOf(dropItem.Item);
			Items.MoveAndSort(oldIdx, newIdx);

			await ItemsChanged.InvokeAsync(Items);
		}
		await ItemDropped.InvokeAsync(dropItem);
	}
}