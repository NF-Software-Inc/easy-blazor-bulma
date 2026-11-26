using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// Creates back and forward buttons for navigation.
/// </summary>
public partial class NavigationButtons : ComponentBase
{
	/// <summary>
	/// Specifies whether to show the back button.
	/// </summary>
	[Parameter]
	public bool DisplayBack { get; set; } = true;

	/// <summary>
	/// Specifies whether to show the forward button.
	/// </summary>
	[Parameter]
	public bool DisplayForward { get; set; } = true;

	/// <summary>
	/// Specifies whether the component will be contained in the navbar.
	/// </summary>
	[Parameter]
	public bool IsNavbarItem { get; set; } = true;

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	[Inject]
	private IJSRuntime JsRuntime { get; init; } = default!;

	private async Task OnBack()
	{
		await JsRuntime.Back();
	}

	private async Task OnForward()
	{
		await JsRuntime.Forward();
	}

	private string FullCssClass
	{
		get
		{
			var css = "";

			if (IsNavbarItem)
				css += " navbar-item";

			return string.Join(' ', css.TrimStart(), CssClass);
		}
	}

	private string? CssClass
	{
		get
		{
			if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("class", out var css) && string.IsNullOrWhiteSpace(Convert.ToString(css, CultureInfo.InvariantCulture)) == false)
				return css.ToString();

			return null;
		}
	}
}
