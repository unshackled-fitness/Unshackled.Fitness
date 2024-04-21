using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Unshackled.Fitness.Core.Models.Charts;

namespace Unshackled.Fitness.Core.Components;

public partial class Chart<T> : IAsyncDisposable where T : struct
{
	[Inject] IJSRuntime JSRuntime { get; set; } = default!;

	[Parameter] public ChartState<T> ChartState { get; set; } = default!;
	[Parameter] public string? Class { get; set; }
	[Parameter] public Dictionary<string, string> CustomLegend { get; set; } = new();
	[Parameter] public bool FormatYaxisAsTime { get; set; } = false;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		ChartState.OnDataLoaded += HandleDataLoaded;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await JSRuntime.InvokeVoidAsync("setupChart", ChartState.Id, ChartState.Config);
			ChartState.SetChartRendered();
		}
	}

	public ValueTask DisposeAsync()
	{
		ChartState.OnDataLoaded -= HandleDataLoaded;
		return ValueTask.CompletedTask;
	}

	protected async void HandleDataLoaded()
	{
		await JSRuntime.InvokeVoidAsync("loadData", ChartState.Id, ChartState.DataSets, FormatYaxisAsTime);
	}
}