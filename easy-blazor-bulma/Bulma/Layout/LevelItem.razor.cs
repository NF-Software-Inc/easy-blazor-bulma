using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Creates an item within a level.
/// </summary>
/// <remarks>
/// There is 1 additional attribute that can be used: a-class. It will apply a CSS class to the resulting element as per its name.
/// <see href="https://bulma.io/documentation/layout/level/">Bulma Documentation</see>
/// </remarks>
public partial class LevelItem : ComponentBase
{
	/// <summary>
	/// The text to display above the title of the level item.
	/// </summary>
	[Parameter]
	public string? Heading { get; set; }

	/// <summary>
	/// The text to display as the main part of the level item.
	/// </summary>
	[Parameter]
	public string? Title { get; set; }

	/// <summary>
	/// Creates a link to the provided URL on the level item.
	/// </summary>
	[Parameter]
	public string? Url { get; set; }

	/// <summary>
	/// The content to display within the level item.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private readonly string[] Filter = ["class", "a-class"];

	private string MainCssClass => string.Join(' ', "level-item", AdditionalAttributes.GetClass("class"));
	private string? UrlCssClass => AdditionalAttributes.GetClass("a-class");
}
