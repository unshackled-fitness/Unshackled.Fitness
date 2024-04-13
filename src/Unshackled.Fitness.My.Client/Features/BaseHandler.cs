using System.Net.Http.Json;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features;

public abstract class BaseHandler
{
	protected readonly HttpClient httpClient;
	protected readonly string baseUrl;

	protected const string appUrlBase = "/api/";

	public BaseHandler(HttpClient httpClient, string relativeUrl)
	{
		this.httpClient = httpClient;
		if (relativeUrl.EndsWith("/"))
			this.baseUrl = $"{appUrlBase}{relativeUrl}";
		else
			this.baseUrl = $"{appUrlBase}{relativeUrl}/";
	}

	protected async Task<TOut?> GetResultAsync<TOut>(string url)
	{
		try
		{
			var response = await httpClient.GetAsync(url);
			return await ReadContent<TOut>(response);
		}
		catch
		{
			return default;
		}
	}
	protected async Task<CommandResult> PostMultipartFormDataToCommandResultAsync(string url, MultipartFormDataContent data)
	{
		try
		{
			var response = await httpClient.PostAsync(url, data);
			return await ReadContent<CommandResult>(response) ?? new CommandResult(false, response.ReasonPhrase);
		}
		catch
		{
			return new CommandResult(false, "Server Connection Error");
		}
	}

	protected async Task<TOut?> PostToResultAsync<TIn, TOut>(string url, TIn data)
	{
		try
		{
			var response = await httpClient.PostAsJsonAsync(url, data);
			return await ReadContent<TOut>(response);
		}
		catch
		{
			return default;
		}
	}

	protected async Task<CommandResult> PostToCommandResultAsync<TIn>(string url, TIn data)
	{
		try
		{
			var response = await httpClient.PostAsJsonAsync(url, data);
			return await ReadContent<CommandResult>(response) ?? new CommandResult(false, response.ReasonPhrase);
		}
		catch
		{
			return new CommandResult(false, "Server Connection Error");
		}
	}

	protected async Task<CommandResult<TOut>> PostToCommandResultAsync<TIn, TOut>(string url, TIn data)
	{
		try
		{
			var response = await httpClient.PostAsJsonAsync(url, data);
			return await ReadContent<CommandResult<TOut>>(response) ?? new CommandResult<TOut>(false, response.ReasonPhrase);
		}
		catch
		{
			return new CommandResult<TOut>(false, "Server Connection Error");
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
