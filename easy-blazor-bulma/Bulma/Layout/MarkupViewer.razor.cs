using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace easy_blazor_bulma;

/// <summary>
/// Converts the provided content into an HTML markup string and displays it.
/// </summary>
public partial class MarkupViewer : ComponentBase
{
	/// <summary>
	/// The HTML encoded content to display.
	/// </summary>
	[EditorRequired]
	[Parameter]
	public required string Content { get; set; }

	/// <summary>
	/// Specifies whether to replace line breaks in the content with <br /> tags.
	/// </summary>
	[Parameter]
	public bool ReplaceLineBreaks { get; set; }

	/// <summary>
	/// Specifies whether to strip HTML comments from the content.
	/// </summary>
	[Parameter]
	public bool StripComments { get; set; }

	/// <summary>
	/// Specifies whether to strip script tags from the content.
	/// </summary>
	[Parameter]
	public bool StripScripts { get; set; }

	/// <summary>
	/// A function that can be provided to sanitize the HTML content before rendering. This allows for custom sanitization logic to be applied to the content.
	/// </summary>
	[Parameter]
	public Func<string, string>? SanitzeHtml { get; set; }

	private MarkupString? Display;

	/// <inheritdoc />
	public async override Task SetParametersAsync(ParameterView parameters)
	{
		var changed = parameters.TryGetValue<string>(nameof(Content), out var updated) && updated != Content;

		await base.SetParametersAsync(parameters);

		if (changed)
		{
			if (ReplaceLineBreaks)
				updated = MatchLineBreaks().Replace(updated!, "<br />");

			if (StripComments)
				updated = MatchHtmlComments().Replace(updated!, "");

			if (StripScripts)
				updated = MatchScripts().Replace(updated!, "");

			if (SanitzeHtml != null)
				updated = SanitzeHtml(updated!);

			Display = new MarkupString(updated!);
		}
	}

	/// <summary>
	/// Matches comments in HTML or XML documents.
	/// </summary>
	[GeneratedRegex(@"<!--[\s\S]*?-->", RegexOptions.Singleline, 1_000)]
	private static partial Regex MatchHtmlComments();

	/// <summary>
	/// Matches line breaks for Windows or Linux line ending styles.
	/// </summary>
	[GeneratedRegex(@"\r?\n|\r", RegexOptions.Multiline, 1_000)]
	private static partial Regex MatchLineBreaks();

	/// <summary>
	/// Matches script tags in HTML documents, including their content.
	/// </summary>
	[GeneratedRegex(@"<script\b[^>]*>(?:[\s\S]*?</script>)|<script\b[^>]*/>", RegexOptions.IgnoreCase | RegexOptions.Singleline, 1_000)]
	private static partial Regex MatchScripts();
}
