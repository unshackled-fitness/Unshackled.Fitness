using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.Core.Components;

public partial class ListView<TItem> where TItem : new()
{
	[Parameter] public string? Class { get; set; }
	[Parameter] public RenderFragment? EmptyListRenderer { get; set; }
	[Parameter] public bool IsLoading { get; set; } = false;
	[Parameter] public RenderFragment<RowContext<TItem>>? ItemRenderer { get; set; }
	[Parameter] public List<TItem> Items { get; set; } = new();
	[Parameter] public int Page { get; set; }
	[Parameter] public EventCallback<int> PageSelected { get; set; }
	[Parameter] public int PageSize { get; set; }
	[Parameter] public PagingPositions PagingPosition { get; set; }
	[Parameter] public int TotalItems { get; set; }
	[Parameter] public bool UseDividers { get; set; } = true;

	protected string ListClass => new CssBuilder("list-view")
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