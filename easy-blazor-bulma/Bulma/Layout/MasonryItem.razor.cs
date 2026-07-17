using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Wraps content as a Masonry layout item.
/// </summary>
public partial class MasonryItem : ComponentBase
{
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

    private readonly string[] Filter = ["class"];

	private string MainCssClass => string.Join(' ', "masonry-item", AdditionalAttributes.GetValue("class"));
}
