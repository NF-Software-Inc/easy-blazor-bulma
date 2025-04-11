using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma;

/// <summary>
/// Base class to create various buttons with.
/// </summary>
/// <remarks>
/// <see href="https://bulma.io/documentation/elements/button/">Bulma Documentation</see>
/// </remarks>
public abstract class ButtonBase : ComponentBase
{
    /// <summary>
    /// The text to display within the button.
    /// </summary>
    [Parameter]
	[Required]
	public required virtual string DisplayText { get; set; }

    /// <summary>
    /// An icon to display to the left of the text.
    /// </summary>
    [Parameter]
    public virtual string? Icon { get; set; }

    /// <summary>
    /// The background color to apply to the button.
    /// </summary>
    [Parameter]
    public virtual BulmaColors Color { get; set; }

    /// <summary>
    /// Any additional attributes applied directly to the component.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Contains HTTP attributes to remove from those applied to the resulting component.
    /// </summary>
	protected virtual string[] Filter { get; } = new[] { "class" };

	/// <summary>
	/// The full CSS class to assign to the button.
	/// </summary>
	protected string MainCssClass
    {
        get
        {
            var css = "button";

            if (Color != BulmaColors.Default)
                css += ' ' + BulmaColorHelper.GetColorCss(Color);

            return string.Join(' ', css, AdditionalAttributes.GetClass("class"));
        }
    }
}
