using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Wraps content as a Masonry layout item.
/// </summary>
public partial class MasonryItem : ComponentBase
{
	/// <summary>
	/// Default CSS class used by <see cref="MasonryItem"/> elements.
	/// </summary>
	public const string DefaultCssClass = "masonry-item";

	/// <summary>
	/// Default selector used by <see cref="Masonry"/> to target <see cref="MasonryItem"/> elements.
	/// </summary>
	public const string DefaultSelector = "." + DefaultCssClass;

	/// <summary>
	/// Content rendered inside the Masonry item.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Additional attributes applied to the root item element.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// CSS class filter to prevent duplicate "class" attributes when combining default and additional classes.
    /// </summary>
    private readonly string[] Filter = ["class"];

    /// <summary>
    /// Gets the combined CSS class for the Masonry item, including the default "masonry-item" class and any additional classes provided via AdditionalAttributes.
    /// </summary>
	private string MainCssClass => string.Join(' ', DefaultCssClass, AdditionalAttributes.GetValue("class"));
}