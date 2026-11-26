using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Displays an error message detailing an Unknown response code.
/// </summary>
public partial class Unknown : HttpErrorBase
{
	/// <inheritdoc/>
	[Parameter]
	public override string? Icon { get; set; } = "help";

	/// <inheritdoc/>
	[Parameter]
	public override string Title { get; set; } = "Unknown";

	/// <inheritdoc/>
	[Parameter]
	public override BulmaColors Color { get; set; } = BulmaColors.Red;

	/// <inheritdoc/>
	[Parameter]
	public override string? Message { get; set; } = "An unknown error occurred.\n" +
		"Contact support for additional help.";
}
