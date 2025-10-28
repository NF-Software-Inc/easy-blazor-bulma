using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// Displays a modal dialog to select a zoom level.
/// </summary>
public partial class ZoomSelector : ComponentBase
{
	/// <summary>
	/// Specifies whether the component will be contained in the navbar.
	/// </summary>
	[Parameter]
	public bool IsNavbarItem { get; set; } = true;

	/// <summary>
	/// The HTML id of the element where the zoom will be applied.
	/// </summary>
	[Parameter]
	public string ElementId { get; set; } = "main-content-section";

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	[Inject]
	private IJSRuntime JsRuntime { get; init; } = default!;

	private bool IsZoomDisplayed;

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

	private async Task ApplyZoom(int zoom)
	{
		_ = await JsRuntime.ApplyStyle(ElementId, "zoom", $"{zoom}%");
		IsZoomDisplayed = false;
	}
}
