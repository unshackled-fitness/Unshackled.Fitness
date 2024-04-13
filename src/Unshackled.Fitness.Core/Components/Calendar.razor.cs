using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Models.Calendars;
using Unshackled.Fitness.Core.Utils;

namespace Unshackled.Fitness.Core.Components;

public partial class Calendar
{
	[Inject] protected IScrollManager ScrollManager { get; set; } = default!;
	[Parameter] public RenderFragment? ChildContent { get; set; }
	[Parameter] public CalendarModel Model { get; set; } = new();

	protected int FirstDayOfWeek => Model.Days.Any()
		? (int)Model.Days.First().Date.DayOfWeek
		: 0;

	protected string GetBlockClass(CalendarBlockModel block)
	{
		string css = "block";
		if (block.IsCentered)
		{
			css += " block-centered";
		}
		if (!block.IsVisible)
		{
			css += " d-none";
		}
		return css;
	}

	protected string GetBlockStyle(CalendarBlockModel block)
	{
		string style = string.Empty;
		if (!string.IsNullOrEmpty(block.Color))
		{
			style += $"background-color: {block.Color};";
		}
		return style;
	}

	protected string GetBlockTextStyle(CalendarBlockModel block)
	{
		string style = string.Empty;
		if (!string.IsNullOrEmpty(block.Color))
		{
			style += $"color: {Calculator.ContrastHexColor(block.Color)};";
		}
		return style;
	}

	protected string GetDayClass(CalendarDayModel day)
	{
		string css = "day";
		if (Model.IsAltDisplay(day))
		{
			css += " day-alt";
		}
		return css;
	}

	protected string GetDayTitle(CalendarDayModel day)
	{
		if (day.Date.Day == 1 || Model.Days.FirstOrDefault() == day)
			return day.Date.ToString("MMM dd");
		else
			return day.Date.ToString("dd");
	}

	protected async Task HandleScrollToBottomClicked()
	{
		await ScrollManager.ScrollIntoViewAsync("#calendar_bottom", ScrollBehavior.Smooth);
	}
}