using System.Net.Http.Json;

namespace Unshackled.Fitness.My.Client.Features;

public abstract class BaseApiHandler
{
	protected readonly HttpClient httpClient;
	protected readonly string baseUrl;

	public BaseApiHandler(IHttpClientFactory httpClientFactory, string relativeUrl)
	{
		this.httpClient = httpClientFactory.CreateClient(AppGlobals.ApiConstants.LibraryApi);
		if (relativeUrl.EndsWith("/"))
			this.baseUrl = relativeUrl;
		else
			this.baseUrl = $"{relativeUrl}/";
	}

	protected async Task<TOut?> GetResultAsync<TOut>(string url)
	{
		try
		{
			var response = await httpClient.GetAsync(url);
			return await ReadContent<TOut>(response);
		}
		catch (HttpRequestException)
		{
			return default;
		}
	}

	protected async Task<TOut?> PostToResultAsync<TIn, TOut>(string url, TIn data)
	{
		try
		{
			var response = await httpClient.PostAsJsonAsync(url, data);
			return await ReadContent<TOut>(response);
		}
		catch (HttpRequestException)
		{
			return default;
		}
	}

	private async Task<TOut?> ReadContent<TOut>(HttpResponseMessage response)
	{
		if (response != null)
		{
			if (response.IsSuccessStatusCode && response.Content != null)
			{
				try
				{
					return await response.Content.ReadFromJsonAsync<TOut>();
				}
				catch
				{
					return default;
				}
			}
			else if (response.Content != null)
			{
				Console.WriteLine(response.Content);
			}
		}
		return default;
	}
}