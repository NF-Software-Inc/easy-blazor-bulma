using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace easy_blazor_bulma;

/// <summary>
/// A composable panel, for compact controls.
/// </summary>
/// <remarks>
/// There are 2 additional attributes that can be used: header-class and content-class. Each of which apply CSS classes to the resulting elements as per their names.
/// <see href="https://bulma.io/documentation/components/panel/">Bulma Documentation</see>
/// </remarks>
public partial class Panel : ComponentBase
{
    /// <summary>
	/// Displays text in a bold bar at the top of the panel.
	/// </summary>
	[Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// An icon to display to the right of the title. Defaults to collapse and expand icons when <see cref="ShowCollapseIcons"/> is true.
    /// </summary>
    [Parameter]
    public string? Icon { get; set; }

    /// <summary>
    /// Sets the color to use for the panel text and background.
    /// </summary>
    [Parameter]
    public BulmaColors Color { get; set; }

    /// <summary>
    /// Specifies whether to use a flex layout for the panel.
    /// </summary>
    [Parameter]
    public bool IsFlex { get; set; }

    /// <summary>
    /// Specifies whether to display the panel content in block format.
    /// </summary>
    [Parameter]
    public bool IsBlock { get; set; } = true;

    /// <summary>
    /// Specifies whether to display the collapse and expand icons on the title bar. Overridden when <see cref="Icon"/> is not null.
    /// </summary>
    [Parameter]
    public bool ShowCollapseIcons { get; set; } = true;

    /// <summary>
    /// Specifies whether the content section of the panel is hidden.
    /// </summary>
    [Parameter]
    public bool IsCollapsed { get; set; }

	/// <summary>
	/// Expression for manual binding to <see cref="IsCollapsed"/>.
	/// </summary>
	[Parameter]
	public Expression<Func<bool>>? IsCollapsedExpression { get; set; }

	/// <summary>
	/// Event that occurs when the collapsed or expanded status of the content section changes.
	/// </summary>
	[Parameter]
    public EventCallback<bool> IsCollapsedChanged { get; set; }

    /// <summary>
    /// A method to run when the title bar is clicked.
    /// </summary>
    [Parameter]
    public EventCallback OnTitleClicked { get; set; }

    /// <summary>
    /// The content to display within the panel.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Any additional attributes applied directly to the component.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private readonly string[] Filter = new[] { "class", "content-class", "header-class" };

    private string MainCssClass
    {
        get
        {
            var css = "panel";

            if (IsFlex)
                css += " is-flex is-flex-direction-column";

            if (Color != BulmaColors.Default)
                css += ' ' + BulmaColorHelper.GetColorCss(Color);

            return string.Join(' ', css, AdditionalAttributes.GetClass("class"));
        }
    }

    private string HeaderCssClass
    {
        get
        {
            var css = "panel-heading is-unselectable py-3";

            if (OnTitleClicked.HasDelegate || ShowCollapseIcons)
                css += " is-clickable";

            return string.Join(' ', css, AdditionalAttributes.GetClass("header-class"));
        }
    }

    private string ContentCssClass
    {
        get
        {
            var css = "panel-block";

            if (IsCollapsed)
                css += " is-hidden";

            if (IsFlex)
                css += " is-flex-grow-1";

            if (IsBlock)
                css += " is-block";

            if (Color != BulmaColors.Default)
                css += ' ' + BulmaColorHelper.GetBackgroundCss(Color, "light");

            return string.Join(' ', css, AdditionalAttributes.GetClass("content-class"));
        }
    }

	private async Task TitleClicked()
    {
        if (OnTitleClicked.HasDelegate)
            await OnTitleClicked.InvokeAsync();
        else if (ShowCollapseIcons)
            await ToggleCollapse();
    }

    private async Task ToggleCollapse()
    {
        IsCollapsed = !IsCollapsed;

        if (IsCollapsedChanged.HasDelegate)
            await IsCollapsedChanged.InvokeAsync(IsCollapsed);
    }
}
