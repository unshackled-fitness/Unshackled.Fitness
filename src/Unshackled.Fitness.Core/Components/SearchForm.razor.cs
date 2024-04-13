using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using Unshackled.Fitness.Core.Extensions;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.Core.Components;

public partial class SearchForm<T> where T : ISearchModel, new()
{
	[Parameter]
	public T? Model { get; set; }

	[Parameter]
	public EventCallback<T> ModelChanged { get; set; }

	[Parameter]
	public EventCallback SearchClicked { get; set; }

	[Parameter]
	public EventCallback ResetClicked { get; set; }

	[Parameter]
	public RenderFragment? SearchFields { get; set; }

	[Parameter]
	public string? Class { get; set; }

	[Parameter]
	public bool Disable { get; set; } = false;

	[Parameter]
	public Breakpoint VerticalButtons { get; set; } = Breakpoint.None;

	[Parameter]
	public bool ForceColumnLayout { get; set; } = false;

	protected string FormClass => new CssBuilder("search")
		.AddClass(ForceColumnLayout ? "search-column-forced" : string.Empty)
		.AddClass(Class)
		.Build();

	protected string ToolbarClass
	{
		get
		{
			if (ForceColumnLayout) return string.Empty;

			var builder = new CssBuilder("search-actions");

			string css = VerticalButtons.MakeCssClass("vertical");
			if (!string.IsNullOrEmpty(css))
				builder.AddClass(css);

			return builder.Build();
		}
	}

	private async Task HandleSubmit()
	{
		await SearchClicked.InvokeAsync();
	}

	private async Task HandleResetClicked()
	{
		await ResetClicked.InvokeAsync();
	}
}