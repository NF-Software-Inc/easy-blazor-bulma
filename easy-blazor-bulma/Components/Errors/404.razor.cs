using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Displays an error message detailing a 404 Not Found response code.
/// </summary>
/// <remarks>
/// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status">Mozilla Reference</see>
/// </remarks>
public partial class _404 : HttpErrorBase
{
	/// <inheritdoc/>
	[Parameter]
	public override string? Icon { get; set; } = "troubleshoot";

	/// <inheritdoc/>
	[Parameter]
	public override string Title { get; set; } = "Not Found";

	/// <inheritdoc/>
	[Parameter]
	public override BulmaColors Color { get; set; } = BulmaColors.Yellow;

	/// <inheritdoc/>
	[Parameter]
	public override string? Message { get; set; } = "Could not find the information you requested.";
}
