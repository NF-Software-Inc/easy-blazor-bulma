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

	[CascadingParameter]
	private Masonry Parent { get; init; } = default!;

	/// <summary>
	/// Additional attributes applied to the root item element.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private readonly string[] Filter = ["class"];

	private string MainCssClass
	{
		get
		{
			var css = "";

			if (Parent.ItemSelector == ".masonry-item")
				css += "masonry-item";

			return string.Join(' ', css, AdditionalAttributes.GetValue("class")); ;
		}
	}
}
