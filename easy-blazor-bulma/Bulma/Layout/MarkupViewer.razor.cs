using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Converts the provided content into an HTML markup string and displays it.
/// </summary>
public partial class MarkupViewer : ComponentBase
{
	/// <summary>
	/// The HTML encoded content to display.
	/// </summary>
	[Parameter]
	public required string Content { get; set; }

	private MarkupString? Display;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		Display = new MarkupString(Content);
	}
}
