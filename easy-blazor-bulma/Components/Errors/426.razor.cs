using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Displays an error message detailing a 426 Upgrade Required code.
/// </summary>
/// <remarks>
/// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status">Mozilla Reference</see>
/// </remarks>
public partial class _426 : HttpErrorBase
{
	/// <inheritdoc/>
	[Parameter]
	public override string? Icon { get; set; } = "browser_updated";

	/// <inheritdoc/>
	[Parameter]
	public override string Title { get; set; } = "Upgrade Required";

	/// <inheritdoc/>
	[Parameter]
	public override BulmaColors Color { get; set; } = BulmaColors.Cyan;

	/// <inheritdoc/>
	[Parameter]
	public override string? Message { get; set; } = "Your application is out of date.\n" +
		"Please ensure you are on the latest version.";
}
