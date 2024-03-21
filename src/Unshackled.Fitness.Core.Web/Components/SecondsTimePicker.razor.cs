using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;

namespace Unshackled.Fitness.Core.Web.Components;

/*
 *  Adapted from MudTimePicker in MudBlazor Component Library to add ability to pick seconds
 *  - https://github.com/MudBlazor/MudBlazor/blob/dev/src/MudBlazor/Components/TimePicker/MudTimePicker.razor.cs
 */

public partial class SecondsTimePicker : MudPicker<TimeSpan?>
{
	private const string format24Hours = "HH:mm:ss";

	public enum OpenToView
	{
		Hours,
		Minutes,
		Seconds
	}

	public SecondsTimePicker() : base(new DefaultConverter<TimeSpan?>())
	{
		Converter.GetFunc = OnGet;
		Converter.SetFunc = OnSet;
		((DefaultConverter<TimeSpan?>)Converter).Format = format24Hours;
		AdornmentIcon = Icons.Material.Filled.AccessTime;
		AdornmentAriaLabel = "Open Time Picker";
	}

	private string OnSet(TimeSpan? timespan)
	{
		if (timespan == null)
			return string.Empty;

		var time = DateTimeOffset.Now.Date.Add(timespan.Value);

		return time.ToString(((DefaultConverter<TimeSpan?>)Converter).Format, Culture);
	}

	private TimeSpan? OnGet(string? value)
	{
		if (string.IsNullOrEmpty(value))
			return null;

		if (DateTime.TryParseExact(value, ((DefaultConverter<TimeSpan?>)Converter).Format, Culture, DateTimeStyles.None, out var time))
		{
			return time.TimeOfDay;
		}
		
		if (DateTime.TryParseExact(value, format24Hours, CultureInfo.InvariantCulture, DateTimeStyles.None,
				out time))
		{
			return time.TimeOfDay;
		}

		HandleParsingError();
		return null;
	}

	private void HandleParsingError()
	{
		const string ParsingErrorMessage = "Not a valid time span";
		Converter.GetError = true;
		Converter.GetErrorMessage = ParsingErrorMessage;
		Converter.OnError?.Invoke(ParsingErrorMessage);
	}

	private OpenToView currentView;
	private string timeFormat = string.Empty;

	internal TimeSpan? TimeIntermediate { get; private set; }

	/// <summary>
	/// First view to show in the MudDatePicker.
	/// </summary>
	[Parameter]
	[Category(CategoryTypes.FormComponent.PickerBehavior)]
	public OpenToView OpenTo { get; set; } = OpenToView.Hours;

	/// <summary>
	/// Choose the edition mode. By default, you can edit hours and minutes.
	/// </summary>
	[Parameter]
	[Category(CategoryTypes.FormComponent.PickerBehavior)]
	public TimeEditMode TimeEditMode { get; set; } = TimeEditMode.Normal;

	/// <summary>
	/// Sets the amount of time in milliseconds to wait before closing the picker. This helps the user see that the time was selected before the popover disappears.
	/// </summary>
	[Parameter]
	[Category(CategoryTypes.FormComponent.PickerBehavior)]
	public int ClosingDelay { get; set; } = 200;

	/// <summary>
	/// If AutoClose is set to true and PickerActions are defined, the hour and the minutes can be defined without any action.
	/// </summary>
	[Parameter]
	[Category(CategoryTypes.FormComponent.PickerBehavior)]
	public bool AutoClose { get; set; }

	/// <summary>
	/// Sets the number interval for minutes.
	/// </summary>
	[Parameter]
	[Category(CategoryTypes.FormComponent.PickerBehavior)]
	public int MinuteSelectionStep { get; set; } = 1;

	/// <summary>
	/// String Format for selected time view
	/// </summary>
	[Parameter]
	[Category(CategoryTypes.FormComponent.Behavior)]
	public string TimeFormat
	{
		get => timeFormat;
		set
		{
			if (timeFormat == value)
				return;

			timeFormat = value;
			if (Converter is DefaultConverter<TimeSpan?> defaultConverter)
				defaultConverter.Format = timeFormat;

			Touched = true;
			SetTextAsync(Converter.Set(_value), false).AndForget();
		}
	}

	/// <summary>
	/// The currently selected time (two-way bindable). If null, then nothing was selected.
	/// </summary>
	[Parameter]
	[Category(CategoryTypes.FormComponent.Data)]
	public TimeSpan? Time
	{
		get => _value;
		set => SetTimeAsync(value, true).AndForget();
	}

	protected async Task SetTimeAsync(TimeSpan? time, bool updateValue)
	{
		if (_value != time)
		{
			Touched = true;
			TimeIntermediate = time;
			_value = time;
			if (updateValue)
				await SetTextAsync(Converter.Set(_value), false);
			UpdateTimeSetFromTime();
			await TimeChanged.InvokeAsync(_value);
			await BeginValidateAsync();
			FieldChanged(_value);
		}
	}

	/// <summary>
	/// Fired when the date changes.
	/// </summary>
	[Parameter] public EventCallback<TimeSpan?> TimeChanged { get; set; }

	protected override Task StringValueChanged(string value)
	{
		Touched = true;
		// Update the time property (without updating back the Value property)
		return SetTimeAsync(Converter.Get(value), false);
	}

	//The last line cannot be tested
	[ExcludeFromCodeCoverage]
	protected override void OnPickerOpened()
	{
		base.OnPickerOpened();
		currentView = TimeEditMode switch
		{
			TimeEditMode.Normal => OpenTo,
			TimeEditMode.OnlyHours => OpenToView.Hours,
			TimeEditMode.OnlyMinutes => OpenToView.Minutes,
			_ => currentView
		};
	}

	protected override void Submit()
	{
		if (GetReadOnlyState())
			return;
		Time = TimeIntermediate;
	}

	public override async void Clear(bool close = true)
	{
		TimeIntermediate = null;
		await SetTimeAsync(null, true);

		if (AutoClose == true)
		{
			Close(false);
		}
	}

	private string GetHourString()
	{
		if (TimeIntermediate == null)
			return "--";
		return Math.Min(23, Math.Max(0, TimeIntermediate.Value.Hours)).ToString(CultureInfo.InvariantCulture);
	}

	private string GetMinuteString()
	{
		if (TimeIntermediate == null)
			return "--";
		return $"{Math.Min(59, Math.Max(0, TimeIntermediate.Value.Minutes)):D2}";
	}

	private string GetSecondString()
	{
		if (TimeIntermediate == null)
			return "--";
		return $"{Math.Min(59, Math.Max(0, TimeIntermediate.Value.Seconds)):D2}";
	}

	private void UpdateTime()
	{
		lastSelectedHour = timeSet.Hour;
		TimeIntermediate = new TimeSpan(timeSet.Hour, timeSet.Minute, timeSet.Second);
		if (PickerVariant == PickerVariant.Static && PickerActions == null || PickerActions != null && AutoClose)
		{
			Submit();
		}
	}

	private void OnHourClick()
	{
		currentView = OpenToView.Hours;
		FocusAsync().AndForget();
	}

	private void OnMinutesClick()
	{
		currentView = OpenToView.Minutes;
		FocusAsync().AndForget();
	}

	private void OnSecondsClick()
	{
		currentView = OpenToView.Seconds;
		FocusAsync().AndForget();
	}

	protected string ToolbarClass =>
	new CssBuilder("mud-picker-timepicker-toolbar")
	  .AddClass($"mud-picker-timepicker-toolbar-landscape", Orientation == Orientation.Landscape && PickerVariant == PickerVariant.Static)
	  .AddClass(Class)
	.Build();

	protected string HoursButtonClass =>
	new CssBuilder("mud-timepicker-button")
	  .AddClass($"mud-timepicker-toolbar-text", currentView != OpenToView.Hours)
	.Build();

	protected string MinuteButtonClass =>
	new CssBuilder("mud-timepicker-button")
	  .AddClass($"mud-timepicker-toolbar-text", currentView != OpenToView.Minutes)
	.Build();

	protected string SecondButtonClass =>
	new CssBuilder("mud-timepicker-button")
	  .AddClass($"mud-timepicker-toolbar-text", currentView != OpenToView.Seconds)
	.Build();

	private string HourDialClass =>
	new CssBuilder("mud-time-picker-hour")
	  .AddClass($"mud-time-picker-dial")
	  .AddClass($"mud-time-picker-dial-out", currentView != OpenToView.Hours)
	  .AddClass($"mud-time-picker-dial-hidden", currentView != OpenToView.Hours)
	.Build();

	private string MinuteDialClass =>
	new CssBuilder("mud-time-picker-minute")
	  .AddClass($"mud-time-picker-dial")
	  .AddClass($"mud-time-picker-dial-out", currentView != OpenToView.Minutes)
	  .AddClass($"mud-time-picker-dial-hidden", currentView != OpenToView.Minutes)
	.Build();

	private string SecondDialClass =>
	new CssBuilder("mud-time-picker-minute")
	  .AddClass($"mud-time-picker-dial")
	  .AddClass($"mud-time-picker-dial-out", currentView != OpenToView.Seconds)
	  .AddClass($"mud-time-picker-dial-hidden", currentView != OpenToView.Seconds)
	.Build();

	private string GetClockPinColor()
	{
		return $"mud-picker-time-clock-pin mud-{Color.ToDescriptionString()}";
	}

	private string GetClockPointerColor()
	{
		if (MouseDown)
			return $"mud-picker-time-clock-pointer mud-{Color.ToDescriptionString()}";
		else
			return $"mud-picker-time-clock-pointer mud-picker-time-clock-pointer-animation mud-{Color.ToDescriptionString()}";
	}

	private string GetClockPointerThumbColor()
	{
		var deg = GetDeg();
		if (deg % 30 == 0)
			return $"mud-picker-time-clock-pointer-thumb mud-onclock-text mud-onclock-primary mud-{Color.ToDescriptionString()}";
		else
			return $"mud-picker-time-clock-pointer-thumb mud-onclock-minute mud-{Color.ToDescriptionString()}-text";
	}

	private string GetNumberColor(int value)
	{
		if (currentView == OpenToView.Hours)
		{
			var h = timeSet.Hour;
			if (h == value)
				return $"mud-clock-number mud-theme-{Color.ToDescriptionString()}";
		}
		else if (currentView == OpenToView.Minutes && timeSet.Minute == value)
		{
			return $"mud-clock-number mud-theme-{Color.ToDescriptionString()}";
		}
		return $"mud-clock-number";
	}

	private double GetDeg()
	{
		double deg = 0;
		if (currentView == OpenToView.Hours)
			deg = timeSet.Hour * 30 % 360;
		if (currentView == OpenToView.Minutes)
			deg = timeSet.Minute * 6 % 360;
		return deg;
	}

	private string GetTransform(double angle, double radius, double offsetX, double offsetY)
	{
		angle = angle / 180 * Math.PI;
		var x = (Math.Sin(angle) * radius + offsetX).ToString("F3", CultureInfo.InvariantCulture);
		var y = ((Math.Cos(angle) + 1) * radius + offsetY).ToString("F3", CultureInfo.InvariantCulture);
		return $"transform: translate({x}px, {y}px);";
	}

	private string GetPointerRotation()
	{
		double deg = 0;
		if (currentView == OpenToView.Hours)
			deg = timeSet.Hour * 30 % 360;
		if (currentView == OpenToView.Minutes)
			deg = timeSet.Minute * 6 % 360;
		if (currentView == OpenToView.Seconds)
			deg = timeSet.Second * 6 % 360;
		return $"rotateZ({deg}deg);";
	}

	private string GetPointerHeight()
	{
		var height = 40;
		if (currentView == OpenToView.Hours)
		{
			if (timeSet.Hour > 0 && timeSet.Hour < 13)
				height = 26;
			else
				height = 40;
		}
		return $"{height}%;";
	}

	private readonly SetTime timeSet = new();
	private int initialHour;
	private int lastSelectedHour;
	private int initialMinute;
	private int initialSecond;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		UpdateTimeSetFromTime();
		currentView = OpenTo;
		initialHour = timeSet.Hour;
		lastSelectedHour = timeSet.Hour;
		initialMinute = timeSet.Minute;
		initialSecond = timeSet.Second;
	}


	private void UpdateTimeSetFromTime()
	{
		if (TimeIntermediate == null)
		{
			timeSet.Hour = 0;
			timeSet.Minute = 0;
			timeSet.Second = 0;
			return;
		}
		timeSet.Hour = TimeIntermediate.Value.Hours;
		timeSet.Minute = TimeIntermediate.Value.Minutes;
		timeSet.Second= TimeIntermediate.Value.Seconds;
	}

	public bool MouseDown { get; set; }

	/// <summary>
	/// Sets Mouse Down bool to true if mouse is inside the clock mask.
	/// </summary>
	private void OnMouseDown(MouseEventArgs e)
	{
		MouseDown = true;
	}

	/// <summary>
	/// Sets Mouse Down bool to false if mouse is inside the clock mask.
	/// </summary>
	private void OnMouseUp(MouseEventArgs e)
	{
		if (MouseDown && currentView == OpenToView.Seconds && timeSet.Second != initialSecond
			|| (currentView == OpenToView.Hours && timeSet.Hour != initialHour && TimeEditMode == TimeEditMode.OnlyHours)
			|| (currentView == OpenToView.Minutes && timeSet.Minute != initialMinute && TimeEditMode == TimeEditMode.OnlyMinutes))
		{
			MouseDown = false;
			SubmitAndClose();
		}

		MouseDown = false;

		if (currentView == OpenToView.Hours && timeSet.Hour != initialHour && TimeEditMode == TimeEditMode.Normal)
		{
			currentView = OpenToView.Minutes;
		}
		else if (currentView == OpenToView.Minutes && timeSet.Minute != initialMinute && TimeEditMode == TimeEditMode.Normal)
		{
			currentView = OpenToView.Seconds;
		}
	}

	/// <summary>
	/// If MouseDown is true enables "dragging" effect on the clock pin/stick.
	/// </summary>
	private void OnMouseOverHour(int value)
	{
		if (MouseDown)
		{
			timeSet.Hour = value;
			UpdateTime();
		}
	}

	/// <summary>
	/// On click for the hour "sticks", sets the hour.
	/// </summary>
	private void OnMouseClickHour(int value)
	{
		timeSet.Hour = value;

		if (currentView == OpenToView.Hours
			|| timeSet.Hour != lastSelectedHour)
		{
			UpdateTime();
		}

		if (TimeEditMode == TimeEditMode.Normal)
		{
			currentView = OpenToView.Minutes;
		}
		else if (TimeEditMode == TimeEditMode.OnlyHours)
		{
			SubmitAndClose();
		}
	}

	/// <summary>
	/// On mouse over for the minutes "sticks", sets the minute.
	/// </summary>
	private void OnMouseOverMinute(int value)
	{
		if (MouseDown)
		{
			value = RoundToStepInterval(value);
			timeSet.Minute = value;
			UpdateTime();
		}
	}

	/// <summary>
	/// On click for the minute "sticks", sets the minute.
	/// </summary>
	private void OnMouseClickMinute(int value)
	{
		value = RoundToStepInterval(value);
		timeSet.Minute = value;
		UpdateTime();

		if (TimeEditMode == TimeEditMode.Normal)
		{
			currentView = OpenToView.Seconds;
		}
		else
		{
			SubmitAndClose();
		}
	}

	/// <summary>
	/// On mouse over for the second "sticks", sets the second.
	/// </summary>
	private void OnMouseOverSecond(int value)
	{
		if (MouseDown)
		{
			value = RoundToStepInterval(value);
			timeSet.Second = value;
			UpdateTime();
		}
	}

	/// <summary>
	/// On click for the second "sticks", sets the second.
	/// </summary>
	private void OnMouseClickSecond(int value)
	{
		value = RoundToStepInterval(value);
		timeSet.Second = value;
		UpdateTime();
		SubmitAndClose();
	}

	private int RoundToStepInterval(int value)
	{
		if (MinuteSelectionStep > 1) // Ignore if step is less than or equal to 1
		{
			var interval = MinuteSelectionStep % 60;
			value = (value + interval / 2) / interval * interval;
			if (value == 60) // For when it rounds up to 60
			{
				value = 0;
			}
		}
		return value;
	}

	protected async void SubmitAndClose()
	{
		if (PickerActions == null || AutoClose)
		{
			Submit();

			if (PickerVariant != PickerVariant.Static)
			{
				await Task.Delay(ClosingDelay);
				Close(false);
			}
		}
	}

	protected override void HandleKeyDown(KeyboardEventArgs obj)
	{
		if (GetDisabledState() || GetReadOnlyState())
			return;
		base.HandleKeyDown(obj);
		switch (obj.Key)
		{
			case "ArrowRight":
				if (IsOpen)
				{
					if (obj.CtrlKey == true)
					{
						ChangeHour(1);
					}
					else if (obj.ShiftKey == true)
					{
						if (timeSet.Minute > 55)
						{
							ChangeHour(1);
						}
						ChangeMinute(5);
					}
					else
					{
						if (timeSet.Minute == 59)
						{
							ChangeHour(1);
						}
						ChangeMinute(1);
					}
				}
				break;
			case "ArrowLeft":
				if (IsOpen)
				{
					if (obj.CtrlKey == true)
					{
						ChangeHour(-1);
					}
					else if (obj.ShiftKey == true)
					{
						if (timeSet.Minute < 5)
						{
							ChangeHour(-1);
						}
						ChangeMinute(-5);
					}
					else
					{
						if (timeSet.Minute == 0)
						{
							ChangeHour(-1);
						}
						ChangeMinute(-1);
					}
				}
				break;
			case "ArrowUp":
				if (IsOpen == false && Editable == false)
				{
					IsOpen = true;
				}
				else if (obj.AltKey == true)
				{
					IsOpen = false;
				}
				else if (obj.ShiftKey == true)
				{
					ChangeHour(5);
				}
				else
				{
					ChangeHour(1);
				}
				break;
			case "ArrowDown":
				if (IsOpen == false && Editable == false)
				{
					IsOpen = true;
				}
				else if (obj.ShiftKey == true)
				{
					ChangeHour(-5);
				}
				else
				{
					ChangeHour(-1);
				}
				break;
			case "Escape":
				ReturnTimeBackUp();
				break;
			case "Enter":
			case "NumpadEnter":
				if (!IsOpen)
				{
					Open();
				}
				else
				{
					Submit();
					Close();
					_inputReference?.SetText(Text);
				}
				break;
			case " ":
				if (!Editable)
				{
					if (!IsOpen)
					{
						Open();
					}
					else
					{
						Submit();
						Close();
						_inputReference?.SetText(Text);
					}
				}
				break;
		}

		StateHasChanged();
	}

	protected void ChangeMinute(int val)
	{
		currentView = OpenToView.Minutes;
		timeSet.Minute = (timeSet.Minute + val + 60) % 60;
		UpdateTime();
	}

	protected void ChangeHour(int val)
	{
		currentView = OpenToView.Hours;
		timeSet.Hour = (timeSet.Hour + val + 24) % 24;
		UpdateTime();
	}

	protected void ReturnTimeBackUp()
	{
		if (Time == null)
		{
			TimeIntermediate = null;
		}
		else
		{
			timeSet.Hour = Time.Value.Hours;
			timeSet.Minute = Time.Value.Minutes;
			UpdateTime();
		}
	}

	private class SetTime
	{
		public int Hour { get; set; }

		public int Minute { get; set; }

		public int Second { get; set; }
	}
}