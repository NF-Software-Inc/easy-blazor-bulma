using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Displays an error message detailing a 405 Method Not Allowed response code.
/// </summary>
/// <remarks>
/// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status">Mozilla Reference</see>
/// </remarks>
public partial class _405 : HttpErrorBase
{
	/// <inheritdoc/>
	[Parameter]
	public override string? Icon { get; set; } = "disabled_visible";

	/// <inheritdoc/>
	[Parameter]
	public override string Title { get; set; } = "Method Not Allowed";

	/// <inheritdoc/>
	[Parameter]
	public override BulmaColors Color { get; set; } = BulmaColors.Red;

	/// <inheritdoc/>
	[Parameter]
	public override string? Message { get; set; } = "Your request cannot be completed.";
}
