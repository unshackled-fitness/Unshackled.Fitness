using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MudBlazor;
using MudBlazor.Services;
using Unshackled.Fitness.Core.Configuration;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Components;
using Unshackled.Fitness.My.Extensions;
using Unshackled.Fitness.My.Features;
using Unshackled.Fitness.My.Middleware;
using Unshackled.Fitness.My.Services;

var builder = WebApplication.CreateBuilder(args);

ConnectionStrings connectionStrings = new ConnectionStrings
{
	DefaultDatabase = builder.Configuration.GetConnectionString("DefaultDatabase") ?? string.Empty
};
builder.Services.AddSingleton(connectionStrings);

SiteConfiguration siteConfig = new();
builder.Configuration.GetSection("SiteConfiguration").Bind(siteConfig);
builder.Services.AddSingleton(siteConfig);

HashIdConfiguration hashConfig = new();
builder.Configuration.GetSection("HashIdConfiguration").Bind(hashConfig);
HashIdSettings.Configure(hashConfig.Alphabet, hashConfig.Salt, hashConfig.MinLength);

SmtpSettings smtpSettings = new();
builder.Configuration.GetSection("SmtpSettings").Bind(smtpSettings);
builder.Services.AddSingleton(smtpSettings);

DbConfiguration dbConfig = new DbConfiguration();
builder.Configuration.GetSection("DbConfiguration").Bind(dbConfig);
builder.Services.AddSingleton(dbConfig);

switch (dbConfig.DatabaseType?.ToLower())
{
	case DbConfiguration.MSSQL:
		builder.Services.AddDbContext<BaseDbContext, MsSqlServerDbContext>();
		break;
	case DbConfiguration.MYSQL:
		builder.Services.AddDbContext<BaseDbContext, MySqlServerDbContext>();
		break;
	case DbConfiguration.POSTGRESQL:
		builder.Services.AddDbContext<BaseDbContext, PostgreSqlServerDbContext>();
		break;
	default:
		break;
}
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingServerAuthenticationStateProvider>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
	{
		options.DefaultScheme = IdentityConstants.ApplicationScheme;
		options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
	})
	.AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(o =>
{
	o.LoginPath = "/account/login";
	o.LogoutPath = "/account/logout";
	o.ExpireTimeSpan = TimeSpan.FromDays(14);
	o.SlidingExpiration = true;
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(3));

builder.Services.AddIdentityCore<UserEntity>(options => {
	options.SignIn.RequireConfirmedAccount = siteConfig.RequireConfirmedAccount;
	options.Password.RequireDigit = siteConfig.PasswordStrength.RequireDigit;
	options.Password.RequireLowercase = siteConfig.PasswordStrength.RequireLowercase;
	options.Password.RequireNonAlphanumeric = siteConfig.PasswordStrength.RequireNonAlphanumeric;
	options.Password.RequireUppercase = siteConfig.PasswordStrength.RequireUppercase;
	options.Password.RequiredLength = siteConfig.PasswordStrength.RequiredLength;
	options.Password.RequiredUniqueChars = siteConfig.PasswordStrength.RequiredUniqueChars;
})
	.AddEntityFrameworkStores<BaseDbContext>()
	.AddSignInManager()
	.AddDefaultTokenProviders();

builder.Services.AddRazorComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();

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

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies([
			Assembly.GetExecutingAssembly(),
			typeof(BaseController).Assembly
		]));
builder.Services.AddAutoMapper([
	Assembly.GetExecutingAssembly(),
			typeof(BaseController).Assembly
]);
builder.Services.AddValidatorsFromAssemblies([
	Assembly.GetExecutingAssembly(),
			typeof(BaseController).Assembly
]);

builder.Services.TryAddScoped<IWebAssemblyHostEnvironment, ServerHostEnvironment>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRenderStateService, RenderStateService>();
builder.Services.AddSingleton<IEmailSender<UserEntity>, SmtpService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseMiddleware<AuthorizedMemberMiddleware>();

app.MapRazorComponents<App>()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(Unshackled.Fitness.My.Client._Imports).Assembly);
// Add additional endpoints required by the Identity /account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapControllers();

app.Run();
