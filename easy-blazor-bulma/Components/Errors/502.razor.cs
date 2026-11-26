using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Displays an error message detailing a 502 Bad Gateway response code.
/// </summary>
/// <remarks>
/// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status">Mozilla Reference</see>
/// </remarks>
public partial class _502 : HttpErrorBase
{
	/// <inheritdoc/>
	[Parameter]
	public override string? Icon { get; set; } = "router";

	/// <inheritdoc/>
	[Parameter]
	public override string Title { get; set; } = "Bad Gateway";

	/// <inheritdoc/>
	[Parameter]
	public override BulmaColors Color { get; set; } = BulmaColors.Yellow;

	/// <inheritdoc/>
	[Parameter]
	public override string? Message { get; set; } = "The server received an invalid response.";
}
