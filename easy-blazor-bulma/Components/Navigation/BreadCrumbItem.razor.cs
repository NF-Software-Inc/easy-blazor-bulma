using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Use to create a single item within a <see cref="BreadCrumb"/> component.
/// </summary>
/// /// <remarks>
/// There are 2 additional attributes that can be used: icon-class and a-class. Each of which apply CSS classes to the resulting elements as per their names.
/// </remarks>
public partial class BreadCrumbItem : ComponentBase, IDisposable
{
    /// <summary>
    /// The text to show for this item.
    /// </summary>
    [Parameter]
    public string? DisplayText { get; set; }

    /// <summary>
    /// The URL to redirect to when clicking this item.
    /// </summary>
    [Parameter]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// An icon to display to the left of this item.
    /// </summary>
    [Parameter]
    public string? Icon { get; set; }

    [CascadingParameter]
    private BreadCrumb Parent { get; set; } = default!;

    /// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    internal readonly string[] Filter = new[] { "class", "a-class", "icon-class" };

    internal int Index;
	internal bool IsLast;

    internal string MainCssClass
    {
        get
        {
			var css = "";

			if (IsLast)
				css += " is-active";

			return string.Join(' ', css, AdditionalAttributes.GetValue("class"));
		}
    }

    internal string? LinkCssClass => AdditionalAttributes.GetValue("a-class");

    internal string IconCssClass => string.Join(' ', "material-icons", AdditionalAttributes.GetValue("icon-class"));

    /// <inheritdoc/>
	protected override void OnInitialized()
    {
        Parent.AddChild(this);
    }

    /// <inheritdoc/>
	public void Dispose()
    {
        Parent.RemoveChild(this);
        GC.SuppressFinalize(this);
    }
}
