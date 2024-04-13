using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.Core.Components;

public partial class GroupedGrid<TGroup, TItem>
	where TGroup : ISortableGroup, new()
	where TItem : IGroupedSortable, new()
{
	[Parameter] public string? Class { get; set; }
	[Parameter] public bool IsLoading { get; set; } = false;
	[Parameter] public int GridSpacing { get; set; } = 2;
	[Parameter] public Justify GridJustify { get; set; } = Justify.FlexStart;
	[Parameter] public List<TGroup> Groups { get; set; } = new();
	[Parameter] public List<TItem> Items { get; set; } = new();
	[Parameter] public int Page { get; set; }
	[Parameter] public int PageSize { get; set; }
	[Parameter] public int TotalItems { get; set; }
	[Parameter] public EventCallback<int> PageSelected { get; set; }
	[Parameter] public RenderFragment<TGroup>? GroupRenderer { get; set; }
	[Parameter] public RenderFragment<TItem>? ItemRenderer { get; set; }
	[Parameter] public RenderFragment? EmptyListRenderer { get; set; }
	[Parameter] public bool ShowSingleGroupTitle { get; set; } = false;

	protected string ViewClass => new CssBuilder("list-view")
		.AddClass(Class)
		.Build();

	protected async Task HandlePageSelected(int page)
	{
		if (page != Page)
		{
			await PageSelected.InvokeAsync(page);
		}
	}
}