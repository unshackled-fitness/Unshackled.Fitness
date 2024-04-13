using System.Reflection;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Unshackled.Fitness.Core.Configuration;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client;
using Unshackled.Fitness.My.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

ClientConfiguration clientConfig = new();
builder.Configuration.GetSection("ClientConfiguration").Bind(clientConfig);
builder.Services.AddSingleton(clientConfig);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services.AddTransient<CookieHandler>();

// Local API Calls
builder.Services
    .AddHttpClient(AppGlobals.ApiConstants.LocalApi, client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CookieHandler>()
	.AddHttpMessageHandler<HttpStatusCodeHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient(AppGlobals.ApiConstants.LocalApi));

// External API Calls
string libApiUrl = clientConfig.LibraryApiUrl ?? builder.HostEnvironment.BaseAddress;
builder.Services
	.AddHttpClient(AppGlobals.ApiConstants.LibraryApi, client => client.BaseAddress = new Uri(libApiUrl));

builder.Services.AddMudServices(config =>
{
	config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;

	config.SnackbarConfiguration.PreventDuplicates = false;
	config.SnackbarConfiguration.NewestOnTop = false;
	config.SnackbarConfiguration.ShowCloseIcon = true;
	config.SnackbarConfiguration.VisibleStateDuration = 3000;
	config.SnackbarConfiguration.HideTransitionDuration = 200;
	config.SnackbarConfiguration.ShowTransitionDuration = 200;
	config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(new Assembly[] {
			Assembly.GetExecutingAssembly(),
			typeof(AppState).Assembly
		}));

builder.Services.AddScoped<IRenderStateService, RenderStateService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AppState>();
builder.Services.AddScoped<HttpStatusCodeHandler>();
builder.Services.AddScoped<ILocalStorage, LocalStorage>();

await builder.Build().RunAsync();
