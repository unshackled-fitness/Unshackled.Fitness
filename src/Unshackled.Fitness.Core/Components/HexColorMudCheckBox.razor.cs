using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Unshackled.Fitness.Core.Components;

public partial class HexColorMudCheckBox<T> : MudCheckBox<T>
{
	private readonly string elementId = string.Concat("checkbox", Guid.NewGuid().ToString().AsSpan(0, 8));

	[Parameter]
	public string? HexColor { get; set; }

	protected string GetCheckboxStyle()
	{
		if (!string.IsNullOrEmpty(HexColor))
		{
			return $"color: {HexColor}";
		}

		return string.Empty;
	}

	private string GetIcon()
	{
		return BoolValue switch
		{
			true => CheckedIcon,
			false => UncheckedIcon,
			_ => IndeterminateIcon
		};
	}
}