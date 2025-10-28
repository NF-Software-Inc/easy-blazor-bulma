using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// The famous media object prevalent in social media interfaces, but useful in any context.
/// </summary>
/// <remarks>
/// <see href="https://bulma.io/documentation/layout/media-object/">Bulma Documentation</see>
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

	private string MainCssClass => string.Join(' ', "media", AdditionalAttributes.GetClass("class"));
    private string LeftCssClass => string.Join(' ', "media-left", AdditionalAttributes.GetClass("left-class"));
	private string ImageCssClass => string.Join(' ', "image is-64x64", AdditionalAttributes.GetClass("image-class"));
	private string ContentCssClass => string.Join(' ', "media-content", AdditionalAttributes.GetClass("content-class"));
}
