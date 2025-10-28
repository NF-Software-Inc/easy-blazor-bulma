using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Creates an imposing hero banner to showcase something.
/// </summary>
/// <remarks>
/// There are 3 additional attributes that can be used: header-class, body-class, and foot-class. Each of which apply CSS classes to the resulting elements as per their names.
/// <see href="https://bulma.io/documentation/layout/hero/">Bulma Documentation</see>
/// </remarks>
public partial class Hero : ComponentBase
{
	/// <summary>
	/// The text to use as the main title for the hero.
	/// </summary>
	[Parameter]
	public string? Title { get; set; }

	/// <summary>
	/// The text to use as the subtitle for the hero.
	/// </summary>
	[Parameter]
	public string? Subtitle { get; set; }

	/// <summary>
	/// Sets the color to use for the hero background.
	/// </summary>
	[Parameter]
	public BulmaColors Color { get; set; }

	/// <summary>
	/// The content to display in the top section of the hero.
	/// </summary>
	[Parameter]
	public RenderFragment? HeroHead { get; set; }

	/// <summary>
	/// The content to display in the main section of the hero.
	/// </summary>
	[Parameter]
	public RenderFragment? HeroBody { get; set; }

	/// <summary>
	/// The content to display in the bottom section of the hero.
	/// </summary>
	[Parameter]
	public RenderFragment? HeroFoot { get; set; }

	/// <summary>
	/// Fallback for content to display within the hero.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private readonly string[] Filter = ["class", "header-class", "body-class", "foot-class"];

	private string MainCssClass
	{
		get
		{
			var css = "hero";

			if (Color != BulmaColors.Default)
				css += ' ' + BulmaColorHelper.GetColorCss(Color);

			return string.Join(' ', css, AdditionalAttributes.GetClass("class"));
		}
	}

	private string HeaderCssClass => string.Join(' ', "hero-head", AdditionalAttributes.GetClass("header-class"));
	private string BodyCssClass => string.Join(' ', "hero-body", AdditionalAttributes.GetClass("body-class"));
	private string FootCssClass => string.Join(' ', "hero-foot", AdditionalAttributes.GetClass("foot-class"));
}
