using Microsoft.AspNetCore.Mvc.Routing;

namespace Unshackled.Fitness.My.Features;

[AttributeUsage(AttributeTargets.Class)]
public class ApiRouteAttribute : Attribute, IRouteTemplateProvider
{
	public ApiRouteAttribute()
	{

	}

	public ApiRouteAttribute(string controllerName)
	{
		this.controllerName = controllerName;
	}

	private string controllerName = "[controller]";
	public string Template => $"api/{controllerName}";
	public int? Order => 2;
	public string? Name { get; set; }
}
