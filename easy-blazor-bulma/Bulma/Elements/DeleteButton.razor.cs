using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma;

/// <summary>
/// Creates a button to delete a record with a confirmation modal.
/// </summary>
/// <remarks>
/// <see href="https://bulma.io/documentation/elements/button/">Bulma Documentation</see>
/// </remarks>
public partial class DeleteButton : ButtonBase
{
    /// <summary>
    /// The text to display within the button.
    /// </summary>
    [Parameter]
    [Required]
	public required override string DisplayText { get; set; } = "Delete";

    /// <summary>
    /// An icon to display to the left of the text.
    /// </summary>
    [Parameter]
    public override string? Icon { get; set; } = "delete";

    /// <summary>
    /// The background color to apply to the button.
    /// </summary>
    [Parameter]
    public override BulmaColors Color { get; set; } = BulmaColors.Red;

    /// <summary>
    /// The text to display in the confirmation modal.
    /// </summary>
    [Parameter]
    public string ConfirmationText { get; set; } = "Are you sure you would like to delete this entry?";

    private bool ShowConfirmationModal;
}
