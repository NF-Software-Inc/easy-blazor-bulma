using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Displays an error message detailing a 401 Unauthorized response code.
/// </summary>
/// <remarks>
/// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status">Mozilla Reference</see>
/// </remarks>
public partial class _401 : HttpErrorBase
{
	/// <inheritdoc/>
	[Parameter]
	public override string? Icon { get; set; } = "block";

	/// <inheritdoc/>
	[Parameter]
	public override string Title { get; set; } = "Unauthorized";

	/// <inheritdoc/>
	[Parameter]
	public override BulmaColors Color { get; set; } = BulmaColors.Red;

	/// <inheritdoc/>
	[Parameter]
	public override string? Message { get; set; } = "Your credentials to the requested resource are invalid.\n" +
		"Try logging in to complete the request.";
}
