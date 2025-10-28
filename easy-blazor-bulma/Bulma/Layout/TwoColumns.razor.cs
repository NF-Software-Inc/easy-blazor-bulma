using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// A simple two-column layout component. The left column is smaller than the right column.
/// </summary>
/// <remarks>
/// There are 4 additional attributes that can be used: left-class, left-style, right-class, and right-style. Each of which apply CSS classes or styles to the resulting elements as per their names.
/// </remarks>
public partial class TwoColumns : ComponentBase
{
	/// <summary>
	/// Content to display in the left column.
	/// </summary>
	[Parameter]
	public RenderFragment Left { get; set; } = default!;

	/// <summary>
	/// Content to display in the right column.
	/// </summary>
	[Parameter]
	public RenderFragment Right { get; set; } = default!;

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private readonly string[] Filter = ["class", "left-class", "right-class", "left-style", "right-style"];

	private string MainCssClass => string.Join(' ', "columns is-variable is-1 px-1", AdditionalAttributes.GetClass("class"));

	private string LeftCssClass => string.Join(' ', "column is-4-tablet is-3-desktop is-3-widescreen is-2-fullhd is-1-4k", AdditionalAttributes.GetClass("left-class"));

	private string RightCssClass => string.Join(' ', "column is-8-tablet is-9-desktop is-9-widescreen is-10-fullhd is-11-4k", AdditionalAttributes.GetClass("right-class"));
}
