using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.Core.Web.Components;

public partial class AppFrameBase : BaseComponent, IAsyncDisposable
{
	public const string ParameterThemeProvider = "ThemeProvider";
	
	[Inject] protected IDialogService DialogService { get; set; } = default!;
	[Inject] protected IJSRuntime jsRuntime { get; set; } = default!;
	[Inject] protected IWebAssemblyHostEnvironment Environment { get; set; } = default!;
	[Parameter] public string Title { get; set; } = string.Empty;
	[Parameter] public MudTheme CustomTheme { get; set; } = new AppTheme();
	[Parameter] public bool IsLoading { get; set; } = true;
	[Parameter] public RenderFragment AppBarContent { get; set; } = default!;
	[Parameter] public RenderFragment BodyContent { get; set; } = default!;
	[Parameter] public RenderFragment LogoContent { get; set; } = default!;
	[Parameter] public RenderFragment NavTopContent { get; set; } = default!;
	[Parameter] public RenderFragment NavBottomContent { get; set; } = default!;
	[Parameter] public EventCallback IntializationCompleted { get; set; }
	[Parameter] public EventCallback<Themes> ThemeSwitched { get; set; }
	[Parameter] public RenderFragment MembershipContent { get; set; } = default!;

	protected bool Open { get; set; } = true;
	protected bool SystemIsDark { get; set; } = false;
	protected MudThemeProvider? ThemeProvider { get; set; }
	protected Themes UseTheme { get; set; } = Themes.System;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		State.OnActiveMemberChange += ActiveMemberChanged;
		State.OnThemeChange += StateHasChanged;
		State.OnServerErrorChange += StateHasChanged;

		await IntializationCompleted.InvokeAsync();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && ThemeProvider is not null)
		{
			SystemIsDark = await ThemeProvider.GetSystemPreference();
			StateHasChanged();
		}
	}

	public override ValueTask DisposeAsync()
	{
		State.OnActiveMemberChange -= ActiveMemberChanged;
		State.OnThemeChange -= StateHasChanged;
		State.OnServerErrorChange -= StateHasChanged;
		return base.DisposeAsync();
	}

	protected string GetThemeIcon()
	{
		return UseTheme switch
		{
			Themes.Dark => Icons.Material.Filled.DarkMode,
			Themes.Light => Icons.Material.Filled.LightMode,
			Themes.System => SystemIsDark ? Icons.Material.Filled.DarkMode : Icons.Material.Filled.LightMode,
			_ => Icons.Material.Filled.LightMode
		};
	}

	public bool GetThemeIsDark()
	{
		return UseTheme switch
		{
			Themes.Dark => true,
			Themes.Light => false,
			Themes.System => SystemIsDark,
			_ => false
		};
	}

	protected void HandleReloadAppClicked()
	{
		NavManager.Refresh(true);
	}

	protected async Task HandleThemeSwitch(Themes setTheme)
	{
		if (UseTheme != setTheme)
		{
			await ThemeSwitched.InvokeAsync(setTheme);
		}
	}

	private void ActiveMemberChanged()
	{
		UseTheme = State.ActiveMember.Settings.AppTheme;
	}

	protected void ToggleDrawer()
	{
		Open = !Open;
	}
}
