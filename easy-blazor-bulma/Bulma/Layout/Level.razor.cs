using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Creates a multi-purpose horizontal level, which can contain almost any other element.
/// </summary>
/// <remarks>
/// <see href="https://bulma.io/documentation/layout/level/">Bulma Documentation</see>
/// </remarks>
public partial class Level : ComponentBase
{
	/// <summary>
	/// The content to display within the level.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private readonly string[] Filter = ["class"];

	private string MainCssClass => string.Join(' ', "level", AdditionalAttributes.GetClass("class"));
}
