using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// A simple two-column layout component. The left column is smaller than the right column.
/// </summary>
/// <remarks>
/// <para>
/// There are 4 additional attributes that can be used: left-class, left-style, right-class, and right-style.
/// Each of which apply CSS classes or styles to the resulting elements as per their names.
/// </para>
///
/// <para>
/// By default the <c>is-4-tablet is-3-desktop is-3-widescreen is-2-fullhd is-1-4k</c> column classes are applied for left-class.
/// By default the <c>is-8-tablet is-9-desktop is-9-widescreen is-10-fullhd is-11-4k</c> column classes are applied for right-class.
/// Providing another Bulma column class will suppress the default so it can take effect.
/// </para>
/// </remarks>
public partial class TwoColumns : ComponentBase
{
	/// <summary>
	/// Content to display in the left column.
	/// </summary>
	[EditorRequired]
	[Parameter]
	public required RenderFragment Left { get; set; }

	/// <summary>
	/// Content to display in the right column.
	/// </summary>
	[EditorRequired]
	[Parameter]
	public required RenderFragment Right { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private readonly string[] Filter = ["class", "left-class", "right-class", "left-style", "right-style"];

	private string MainCssClass => string.Join(' ', "columns is-variable is-1 px-1", AdditionalAttributes.GetValue("class"));

	private string LeftCssClass
	{
		get
		{
			var css = string.Join(' ', "column", AdditionalAttributes.GetValue("left-class"));

			if (CssClassHelper.ContainsColumnWidthClass(css) == false)
				css += " is-4-tablet is-3-desktop is-3-widescreen is-2-fullhd is-1-4k";

			return css;
		}
	}

	private string RightCssClass
	{
		get
		{
			var css = string.Join(' ', "column", AdditionalAttributes.GetValue("right-class"));

			if (CssClassHelper.ContainsColumnWidthClass(css) == false)
				css += " is-8-tablet is-9-desktop is-9-widescreen is-10-fullhd is-11-4k";

			return css;
		}
	}
}
