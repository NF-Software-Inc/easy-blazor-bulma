using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma;

/// <summary>
/// Creates a button to reset contents of a form.
/// </summary>
/// <remarks>
/// <see href="https://bulma.io/documentation/elements/button/">Bulma Documentation</see>
/// </remarks>
public partial class ResetButton : ButtonBase
{
    /// <summary>
    /// The text to display within the button.
    /// </summary>
    [Parameter]
	[Required]
	public required override string DisplayText { get; set; } = "Reset";

    /// <summary>
    /// An icon to display to the left of the text.
    /// </summary>
    [Parameter]
    public override string? Icon { get; set; } = "restart_alt";

    /// <summary>
    /// The background color to apply to the button.
    /// </summary>
    [Parameter]
    public override BulmaColors Color { get; set; } = BulmaColors.Yellow;
}
