using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Creates an icon that displayed one of two selected icons based on the provided value.
/// </summary>
/// <remarks>
/// <see href="https://bulma.io/documentation/elements/icon/">Bulma Documentation</see>
/// </remarks>
public partial class BooleanIcon : ComponentBase
{
	/// <summary>
	/// Displays the chosen TrueIcon when true or the chosen FalseIcon when false.
	/// </summary>
	[Parameter]
	public bool Value { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// The icon that is displayed when Value is true.
    /// </summary>
    [Parameter]
    public string TrueIcon { get; set; } = "check_circle";
    /// <summary>
    /// The icon that is displayed when Value is false.
    /// </summary>
    [Parameter]
    public string FalseIcon { get; set; } = "cancel";
    /// <summary>
    /// The color of the icon that is displayed when Value is true.
    /// </summary>
    [Parameter]
	public BulmaColors TrueColor { get; set; } = BulmaColors.Green;
    /// <summary>
    /// The color of the icon that is displayed when Value is false.
    /// </summary>
    [Parameter]
    public BulmaColors FalseColor { get; set; } = BulmaColors.Red;

    private string DefaultTrueIcon = "check_circle";
	private string DefaultFalseIcon = "cancel";
    private BulmaColors DefaultTrueColor = BulmaColors.Green;
    private BulmaColors DefaultFalseColor = BulmaColors.Red;

    private readonly string[] Filter = new[] { "class" };

    private string MainCssClass
	{
		get
		{
			var css = "material-icons";
			css += ' ' + BulmaColorHelper.GetTextCss(Color);
			return string.Join(' ', css, AdditionalAttributes.GetClass("class"));
		}
	}
	private string Icon => Value ? TrueIcon : FalseIcon;
    private BulmaColors Color => Value ? TrueColor : FalseColor;
}
