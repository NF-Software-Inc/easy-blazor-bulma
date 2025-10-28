using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// A simple three-column layout component. The left and right columns are smaller than the middle column.
/// </summary>
/// <remarks>
/// There are 6 additional attributes that can be used: left-class, middle-class, right-class, left-style, middle-style, and right-style. Each of which apply CSS classes or styles to the resulting elements as per their names.
/// </remarks>
public partial class ThreeColumns : ComponentBase
{
	/// <summary>
	/// Content to display in the left column.
	/// </summary>
	[Parameter]
	public required RenderFragment Left { get; set; }

	/// <summary>
	/// Content to display in the middle column.
	/// </summary>
	[Parameter]
	public required RenderFragment Middle { get; set; }

	/// <summary>
	/// Content to display in the right column.
	/// </summary>
	[Parameter]
	public required RenderFragment Right { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private readonly string[] Filter = ["class", "left-class", "middle-class", "right-class", "left-style", "middle-style", "right-style"];

	private string MainCssClass => string.Join(' ', "columns is-variable is-1 px-1", AdditionalAttributes.GetClass("class"));

	private string LeftCssClass => string.Join(' ', "column is-4-tablet is-3-desktop is-3-widescreen is-2-fullhd is-1-4k", AdditionalAttributes.GetClass("left-class"));
	private string MiddleCssClass => string.Join(' ', "column is-4-tablet is-6-desktop is-6-widescreen is-8-fullhd is-10-4k", AdditionalAttributes.GetClass("middle-class"));

	private string RightCssClass => string.Join(' ', "column is-4-tablet is-3-desktop is-3-widescreen is-2-fullhd is-1-4k", AdditionalAttributes.GetClass("right-class"));
}
