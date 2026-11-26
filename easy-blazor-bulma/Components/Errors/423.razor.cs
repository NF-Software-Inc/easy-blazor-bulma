using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Displays an error message detailing a 423 Locked response code.
/// </summary>
/// <remarks>
/// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status">Mozilla Reference</see>
/// </remarks>
public partial class _423 : HttpErrorBase
{
	/// <inheritdoc/>
	[Parameter]
	public override string? Icon { get; set; } = "lock";

	/// <inheritdoc/>
	[Parameter]
	public override string Title { get; set; } = "Locked";

	/// <inheritdoc/>
	[Parameter]
	public override BulmaColors Color { get; set; } = BulmaColors.Yellow;

	/// <inheritdoc/>
	[Parameter]
	public override string? Message { get; set; } = "The requested resource is locked.";
}
