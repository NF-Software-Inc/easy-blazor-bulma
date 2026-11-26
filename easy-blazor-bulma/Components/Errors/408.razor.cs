using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Displays an error message detailing a 408 Request Timeout response code.
/// </summary>
/// <remarks>
/// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status">Mozilla Reference</see>
/// </remarks>
public partial class _408 : HttpErrorBase
{
	/// <inheritdoc/>
	[Parameter]
	public override string? Icon { get; set; } = "hourglass_disabled";

	/// <inheritdoc/>
	[Parameter]
	public override string Title { get; set; } = "Request Timeout";

	/// <inheritdoc/>
	[Parameter]
	public override BulmaColors Color { get; set; } = BulmaColors.Yellow;

	/// <inheritdoc/>
	[Parameter]
	public override string? Message { get; set; } = "Your request timed out waiting for the server to respond.";
}
