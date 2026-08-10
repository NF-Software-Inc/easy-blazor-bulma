using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// The famous media object prevalent in social media interfaces, but useful in any context.
/// </summary>
/// <remarks>
/// <para>
/// There are 3 additional attributes that can be used: left-class, image-class, and content-class.
/// They will apply CSS classes to the resulting elements as per their names.
/// </para>
///
/// <para>
/// By default the <c>is-64x64</c> image class is applied for image-class.
/// Providing another Bulma image class will suppress the default so it can take effect.
/// </para>
///
/// <para>
/// <see href="https://bulma.io/documentation/layout/media-object/">Bulma Documentation</see>
/// </para>
/// </remarks>
public partial class MediaObject : ComponentBase
{
	/// <summary>
	/// The URL of an image to display in the top left of the media object.
	/// </summary>
	[Parameter]
	public string? DisplayImageUrl { get; set; }

    /// <summary>
    /// The content to display within the media object.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Any additional attributes applied directly to the component.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private readonly string[] Filter = ["class", "left-class", "image-class", "content-class"];

	private string MainCssClass => string.Join(' ', "media", AdditionalAttributes.GetValue("class"));
    private string LeftCssClass => string.Join(' ', "media-left", AdditionalAttributes.GetValue("left-class"));
	private string ImageCssClass
	{
		get
		{
			var css = string.Join(' ', "image", AdditionalAttributes.GetValue("image-class"));

			if (CssClassHelper.ContainsImageDimensionClass(css) == false)
				css += " is-64x64";

			return css;
		}
	}
	private string ContentCssClass => string.Join(' ', "media-content", AdditionalAttributes.GetValue("content-class"));
}
