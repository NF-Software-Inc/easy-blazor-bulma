using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// A colored message block, to emphasize part of your page.
/// </summary>
/// <remarks>
/// There are 2 additional attributes that can be used: header-class and body-class. Each of which apply CSS classes to the resulting elements as per their names.
/// <see href="https://bulma.io/documentation/components/message/">Bulma Documentation</see>
/// </remarks>
public partial class Message : ComponentBase
{
	/// <summary>
	/// Displays text in a bold bar at the top of the message.
	/// </summary>
	[Parameter]
	public string? Title { get; set; }

	/// <summary>
	/// Displays a button to hide the message from the display.
	/// </summary>
	[Parameter]
	public bool ShowDeleteButton { get; set; } = true;

	/// <summary>
	/// Specifies whether the message should be hidden from display.
	/// </summary>
	[Parameter]
	public bool IsHidden { get; set; }

    /// <summary>
    /// Sets the color to use for the message text and background.
    /// </summary>
    [Parameter]
    public BulmaColors Color { get; set; }

    /// <summary>
    /// The content to display within the message.
    /// </summary>
    [Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private readonly string[] Filter = new[] { "class", "header-class", "body-class" };

    private string MainCssClass
	{
		get
		{
			var css = "message";

			if (IsHidden)
				css += " is-hidden";

            if (Color != BulmaColors.Default)
                css += ' ' + BulmaColorHelper.GetColorCss(Color);

            return string.Join(' ', css, AdditionalAttributes.GetValue("class"));
        }
	}

    private string HeaderCssClass => string.Join(' ', "message-header", AdditionalAttributes.GetValue("header-class"));

    private string BodyCssClass => string.Join(' ', "message-body", AdditionalAttributes.GetValue("body-class"));

    private void Delete()
	{
		IsHidden = true;
		StateHasChanged();
	}
}
