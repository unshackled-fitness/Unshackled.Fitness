namespace Unshackled.Fitness.Core.Models.Charts;

public class ChartState<T> where T : struct
{
	public string Id { get; private set; } = "chart";
	public object Config { get; private set; } = default!;
	public bool IsRendered { get; private set; }
	public ChartDataSet<T>[] DataSets { get; private set; } = [];

	public event Action? OnDataLoaded;

	public void Configure(string id, object config)
	{
		Id = id;
		Config = config;
	}

	public void SetChartRendered()
	{
		if (!IsRendered)
		{
			IsRendered = true;
			OnDataLoaded?.Invoke();
		}
	}

	public void LoadData(ChartDataSet<T>[] datasets)
	{		
		DataSets = datasets;

		if (IsRendered)
		{
			OnDataLoaded?.Invoke();
		}
	}
}
