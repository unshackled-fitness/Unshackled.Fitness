using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Extensions;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.Core.Components;

public partial class SortableGroupedList<TGroup, TItem>
	where TGroup : ISortableGroupForm, ICloneable, new()
	where TItem : IGroupedSortable, ICloneable, new()
{

	protected const string NewDropZoneId = "_create_new";

	[Parameter] public bool AllowSort { get; set; } = true;
	[Parameter] public string? Class { get; set; }
	[Parameter] public bool DisableSort { get; set; } = false;
	[Parameter] public RenderFragment<TGroup>? DraggableGroupRenderer { get; set; }
	[Parameter] public RenderFragment<TItem>? DraggableItemRenderer { get; set; }
	[Parameter] public string? DropZoneClass { get; set; }
	[Parameter] public RenderFragment? EmptyListRenderer { get; set; }
	[Parameter] public string GroupLabel { get; set; } = "Groups";
	[Parameter] public RenderFragment<TGroup>? GroupRenderer { get; set; }
	[Parameter] public List<TGroup> Groups { get; set; } = new();
	[Parameter] public bool IsLoading { get; set; } = false;
	[Parameter] public EventCallback<bool> IsSortingChanged { get; set; }
	[Parameter] public string? ItemDraggingClass { get; set; }
	[Parameter] public RenderFragment<RowContext<TItem>>? ItemRenderer { get; set; }
	[Parameter] public List<TItem> Items { get; set; } = new();
	[Parameter] public RenderFragment? ListTools { get; set; }
	[Parameter] public RenderFragment? NewZoneRenderer { get; set; }
	[Parameter] public bool ShowSingleGroupTitle { get; set; } = false;
	[Parameter] public EventCallback<SortableGroupResult<TGroup, TItem>> SortOrderChanged { get; set; }
	[Parameter] public HorizontalAlignment ToolbarAlignment { get; set; } = HorizontalAlignment.Start;
	[Parameter] public ToolbarPositions ToolbarPosition { get; set; } = ToolbarPositions.Top;
	[Parameter] public int TotalItems { get; set; }
	[Parameter] public bool UseDividers { get; set; } = true;

	protected List<TGroup> DeletedGroups { get; set; } = new();
	protected bool IsReordering { get; set; } = false;
	protected bool IsReorderingGroups { get; set; }
	protected string ListClass => new CssBuilder("list-view")
		.AddClass(Class)
		.Build();
	protected List<TGroup> SortingGroups { get; set; } = new();
	protected List<TItem> SortingItems { get; set; } = new();
	protected bool IsWorking { get; set;} = false;
	protected string ZoneClass =>
		new CssBuilder("mud-paper")
			.AddClass($"mud-paper-outlined")
			.AddClass($"no-background")
			.AddClass($"mud-elevation-0")
			.AddClass($"py-2 px-4 mb-4")
			.AddClass(DropZoneClass)
		.Build();

	protected void HandleBackClicked()
	{
		IsReorderingGroups = false;
	}

	protected async Task HandleCancelSortClicked()
	{
		IsReordering = false;
		await IsSortingChanged.InvokeAsync(false);
	}

	protected void HandleGroupDropped(MudItemDropInfo<TGroup> dropItem)
	{
		if (dropItem.Item != null)
		{
			int newIdx = dropItem.IndexInZone;
			int oldIdx = dropItem.Item.SortOrder;
			SortingGroups.MoveAndSort(oldIdx, newIdx);

			int idx = 0;
			foreach (var group in SortingGroups)
			{
				var items = SortingItems.Where(x => x.ListGroupSid == group.Sid).ToList();
				foreach (var item in items)
				{
					item.SortOrder = idx;
					idx++;
				}
			}

			SortingItems = SortingItems.OrderBy(x => x.SortOrder).ToList();
		}
	}

	protected void HandleItemDropped(MudItemDropInfo<TItem> dropItem)
	{
		string dropzoneId = dropItem.DropzoneIdentifier;
		if (dropzoneId == NewDropZoneId)
		{
			TGroup newGroup = new()
			{
				Sid = Guid.NewGuid().ToString(),
				SortOrder = Groups.Count,
				IsNew = true
			};
			SortingGroups.Add(newGroup);

			dropzoneId = newGroup.Sid;
		}

		if (dropItem.Item != null)
		{
			dropItem.Item.ListGroupSid = dropzoneId;

			int newIdx = dropItem.IndexInZone;
			int oldIdx = SortingItems.IndexOf(dropItem.Item);
			foreach (var group in SortingGroups)
			{
				if (group.Sid == dropzoneId)
					break;

				newIdx += SortingItems.Where(x => x.ListGroupSid == group.Sid).Count();
			}
			SortingItems.MoveAndSort(oldIdx, newIdx);
		}

		// Remove empty groups
		int i = 0;
		while (i < SortingGroups.Count)
		{
			int itemCount = SortingItems.Where(x => x.ListGroupSid == SortingGroups[i].Sid).Count();
			if (itemCount == 0)
			{
				DeletedGroups.Add(SortingGroups[i]);
				SortingGroups.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}

		// Clear single group name
		if (SortingGroups.Count == 1) 
		{
			SortingGroups[0].Title = string.Empty;
		}
	}

	protected async Task HandleSortClicked()
	{
		DeletedGroups = new();
		SortingGroups = Groups.ConvertAll(x => (TGroup)x.Clone());
		SortingItems = Items.ConvertAll(x => (TItem)x.Clone());
		IsReordering = true;
		await IsSortingChanged.InvokeAsync(true);
	}

	protected void HandleSortGroupClicked()
	{
		IsReorderingGroups = true;
	}

	protected async Task HandleUpdateSortClicked()
	{
		IsWorking = true;
		var result = new SortableGroupResult<TGroup, TItem>
		{
			DeletedGroups = DeletedGroups,
			Groups = SortingGroups,
			Items = SortingItems
		};
		await SortOrderChanged.InvokeAsync(result);
		await IsSortingChanged.InvokeAsync(false);
		IsReordering = false;
		IsWorking = false;
	}
}