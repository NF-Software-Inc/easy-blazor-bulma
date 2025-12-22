using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// An all-around flexible and composable component.
/// </summary>
/// <remarks>
/// There are 4 additional attributes that can be used: header-class, title-class, content-class, and footer-class. Each of which apply CSS classes to the resulting elements as per their names.
/// <see href="https://bulma.io/documentation/components/card/">Bulma Documentation</see>
/// </remarks>
public partial class Card : ComponentBase
{
	/// <summary>
	/// Displays text in a bold bar at the top of the card.
	/// </summary>
	[Parameter]
	public string? Title { get; set; }

	/// <summary>
	/// An icon to display to the right of the title.
	/// </summary>
	[Parameter]
	public string? Icon { get; set; }

	/// <summary>
	/// The URL of an image to display in the within the card.
	/// </summary>
	[Parameter]
	public string? DisplayImageUrl { get; set; }

	/// <summary>
	/// The size or aspect ratio to display the image in the card.
	/// </summary>
	[Parameter]
	public BulmaImageSizes ImageSize { get; set; } = BulmaImageSizes.Is4by3;

	/// <summary>
	/// The content to display within the card.
	/// </summary>
	[Parameter]
	public RenderFragment? CardContent { get; set; }

	/// <summary>
	/// The content to display within the card.
	/// </summary>
	[Parameter]
	public RenderFragment? CardFooter { get; set; }

	/// <summary>
	/// Fallback for content to display within the card. Applies when neither <see cref="CardContent"/> nor <see cref="CardFooter"/> are provided.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private readonly string[] Filter = new[] { "class", "header-class", "title-class", "content-class", "footer-class" };

    private string MainCssClass => string.Join(' ', "card", AdditionalAttributes.GetValue("class"));

    private string HeaderCssClass => string.Join(' ', "card-header", AdditionalAttributes.GetValue("header-class"));

    private string TitleCssClass => string.Join(' ', "card-header-title", AdditionalAttributes.GetValue("title-class"));

	private string FigureCssClass => $"image {BulmaImageHelper.GetImageCss(ImageSize)}";

    private string ContentCssClass => string.Join(' ', "card-content", AdditionalAttributes.GetValue("content-class"));

    private string FooterCssClass => string.Join(' ', "card-footer", AdditionalAttributes.GetValue("footer-class"));
}
