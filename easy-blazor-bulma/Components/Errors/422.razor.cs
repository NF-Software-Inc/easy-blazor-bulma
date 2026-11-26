using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Displays an error message detailing a 422 Unprocessable Entity response code.
/// </summary>
/// <remarks>
/// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status">Mozilla Reference</see>
/// </remarks>
public partial class _422 : HttpErrorBase
{
	/// <inheritdoc/>
	[Parameter]
	public override string? Icon { get; set; } = "error_outline";

	/// <inheritdoc/>
	[Parameter]
	public override string Title { get; set; } = "Unprocessable Entity";

	/// <inheritdoc/>
	[Parameter]
	public override BulmaColors Color { get; set; } = BulmaColors.Red;

	/// <inheritdoc/>
	[Parameter]
	public override string? Message { get; set; } = "Your submission contained one or more errors.\n" +
		"Please try again.";
}
