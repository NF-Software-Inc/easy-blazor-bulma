using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma;

/// <summary>
/// Accepts and displays a text value and truncates it if it is longer than the provided limit.
/// </summary>
public class TruncatedText : ComponentBase
{
	/// <summary>
	/// The text to display.
	/// </summary>
	[Parameter]
	public string? Text { get; set; }

	/// <summary>
	/// The amount of characters to display before truncating the text.
	/// </summary>
	[Parameter]
	[Range(1, int.MaxValue)]
	public int Length { get; set; }

	/// <inheritdoc />
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenElement(0, "span");

		if (string.IsNullOrEmpty(Text))
			builder.AddMarkupContent(1, "&nbsp;");
		else if (Text.Length < Length)
			builder.AddContent(1, Text);
		else
			builder.AddMarkupContent(1, Text[..Length] + "&hellip;");

		builder.CloseElement();
	}
}
