using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma;

/// <summary>
/// Creates a progress bar that displays the provided value or a loading animation.
/// </summary>
/// <remarks>
/// <see href="https://bulma.io/documentation/elements/progress/">Bulma Documentation</see>
/// </remarks>
public partial class ProgressBar : ComponentBase
{
    /// <summary>
    /// The point to bring the bar to in relation to <see cref="Max"/>.
    /// </summary>
    [Parameter]
    public int? Current { get; set; }

    /// <summary>
    /// The value at which the bar will be completely filled.
    /// </summary>
    [Parameter]
    [Range(0, int.MaxValue)]
    public int Max { get; set; } = 100;

    /// <summary>
    /// Sets the fill color for the progress bar.
    /// </summary>
    [Parameter]
    public BulmaColors Color { get; set; }

    /// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private readonly string[] Filter = new[] { "class" };

    private string MainCssClass
    {
        get
        {
            var css = "progress";

            if (Color != BulmaColors.Default)
                css += ' ' + BulmaColorHelper.GetColorCss(Color);

			return string.Join(' ', css, AdditionalAttributes.GetValue("class"));
		}
    }
}
