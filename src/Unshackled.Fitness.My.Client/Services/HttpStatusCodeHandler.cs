using System.Net;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.My.Client.Services;

public class HttpStatusCodeHandler : DelegatingHandler
{
	private readonly ISnackbar snackbar;
	private readonly AppState appState;
	private readonly NavigationManager navManager;

	public HttpStatusCodeHandler(ISnackbar snackbar, AppState state, NavigationManager nav)
	{
		this.snackbar = snackbar;
		appState = state;
		navManager = nav;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		// before sending the request
		var response = await base.SendAsync(request, cancellationToken);

		// after sending the request
		if (!response.IsSuccessStatusCode)
		{
			switch (response.StatusCode)
			{
				case HttpStatusCode.BadRequest:
					snackbar.Add("An invalid value was submitted to the server.", Severity.Error);
					break;
				case HttpStatusCode.Forbidden:
					navManager.NavigateTo("/account/login", true);
					break;
				case HttpStatusCode.InternalServerError:
					appState.SetServerError();
					break;
				case HttpStatusCode.NotFound:
					snackbar.Add("The requested url was not found.", Severity.Error);
					break;
				case HttpStatusCode.Unauthorized:
					navManager.NavigateTo("/account/login", true);
					break;
				default:
					snackbar.Add($"Server returned status code {(int)response.StatusCode} {response.ReasonPhrase}", Severity.Error);
					break;
			}
		}

		return response;
	}
}
