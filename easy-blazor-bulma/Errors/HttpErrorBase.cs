using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Base class to define common parameters and properties for HTTP error messages.
/// </summary>
public abstract class HttpErrorBase : ComponentBase
{
    /// <summary>
    /// An icon to display in the banner for the error message.
    /// </summary>
    public abstract string? Icon { get; set; }

    /// <summary>
    /// Text to display in the banner for the error message.
    /// </summary>
    public abstract string Title { get; set; }

    /// <summary>
    /// The color to display the banner in for the error message.
    /// </summary>
    public abstract BulmaColors Color { get; set; }

    /// <summary>
    /// Text to display in the main content area of the error message.
    /// </summary>
    public abstract string? Message { get; set; }

	/// <summary>
	/// Additional content to display within the error.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// The full CSS class to assign to the error message.
    /// </summary>
    protected string MainCssClass
    {
        get
        {
            return string.Join(' ', "column is-12-tablet is-10-desktop is-10-widescreen", AdditionalAttributes.GetValue("class"));
        }
    }
}
