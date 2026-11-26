using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Displays an error message detailing a 429 Too Many Requests response code.
/// </summary>
/// <remarks>
/// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status">Mozilla Reference</see>
/// </remarks>
public partial class _429 : HttpErrorBase
{
	/// <inheritdoc/>
	[Parameter]
	public override string? Icon { get; set; } = "timer_off";

	/// <inheritdoc/>
	[Parameter]
	public override string Title { get; set; } = "Too Many Requests";

	/// <inheritdoc/>
	[Parameter]
	public override BulmaColors Color { get; set; } = BulmaColors.Yellow;

	/// <inheritdoc/>
	[Parameter]
	public override string? Message { get; set; } = "You have sent too many requests.\n" +
		"Please try again later.";
}
