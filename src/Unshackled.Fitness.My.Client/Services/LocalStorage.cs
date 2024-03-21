using Blazored.LocalStorage;
using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.My.Client.Services;

public class LocalStorage : ILocalStorage
{
	public const int DefaultCacheDurationMinutes = 30;

	protected readonly ILocalStorageService localStorage;
	protected readonly AppState appState;
	protected string basePrefix = string.Empty;

	public LocalStorage(ILocalStorageService localStorage, AppState appState)
	{
		this.localStorage = localStorage;
		this.appState = appState; 
		this.basePrefix = $"uf_{appState.ActiveMember.Sid}_";
	}

	public async Task RemoveItemAsync(string key)
	{
		string prefixedKey = $"{basePrefix}{key}";
		if (await localStorage.ContainKeyAsync(prefixedKey))
			await localStorage.RemoveItemAsync(prefixedKey);
	}

	public async Task<TOut?> GetItemAsync<TOut>(string key)
	{
		try
		{
			string prefixedKey = $"{basePrefix}{key}";
			if (await localStorage.ContainKeyAsync(prefixedKey))
				return await localStorage.GetItemAsync<TOut>(prefixedKey);
			return default;
		}
		catch
		{
			return default;
		}
	}

	public async Task<string?> GetItemAsStringAsync(string key)
	{
		try
		{
			string prefixedKey = $"{basePrefix}{key}";
			if (await localStorage.ContainKeyAsync(prefixedKey))
				return await localStorage.GetItemAsStringAsync(prefixedKey);
			return string.Empty;
		}
		catch
		{
			return string.Empty;
		}
	}

	public async Task SetItemAsync<TIn>(string key, TIn value)
	{
		await SetItemAsync(key, value, CancellationToken.None);
	}

	public async Task SetItemAsync<TIn>(string key, TIn value, CancellationToken cancellationToken)
	{
		await localStorage.SetItemAsync($"{basePrefix}{key}", value, cancellationToken);
	}

	public async Task SetItemAsStringAsync(string key, string value)
	{
		await SetItemAsStringAsync(key, value, CancellationToken.None);
	}

	public async Task SetItemAsStringAsync(string key, string value, CancellationToken cancellationToken)
	{
		await localStorage.SetItemAsStringAsync($"{basePrefix}{key}", value, cancellationToken);
	}
}
