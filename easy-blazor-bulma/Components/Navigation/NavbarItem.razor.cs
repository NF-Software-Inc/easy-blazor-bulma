using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace easy_blazor_bulma;

/// <summary>
/// A link to use in a Navbar menu.
/// </summary>
/// <remarks>
/// There are 2 additional attributes that can be used: link-class and icon-class. Each of which will apply CSS classes to the resulting elements as per their names.
/// <see href="https://bulma.io/documentation/components/navbar/">Bulma Documentation</see>
/// </remarks>
public partial class NavbarItem : ComponentBase
{
    /// <summary>
    /// An icon to display to the left of the title.
    /// </summary>
    [Parameter]
    public string? Icon { get; set; }

	/// <summary>
	/// Specifies whether to make display items smaller on mobile resolutions.
	/// </summary>
	[Parameter]
	public bool CompactDisplay { get; set; }

	/// <inheritdoc cref="NavLink.Match" />
	[Parameter]
	public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;

	/// <summary>
	/// The content to display within the item.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private readonly string[] Filter = new[] { "class", "link-class", "icon-class" };

    private string MainCssClass => string.Join(' ', "navbar-item", AdditionalAttributes.GetValue("class"));

    private string LinkCssClass
	{
		get
		{
			var css = "";

			if (string.IsNullOrWhiteSpace(Icon) == false)
				css += " ml-2";

			if (CompactDisplay && ChildContent != null)
				css += " is-hidden-touch is-hidden-desktop-only is-hidden-widescreen-only";

            return string.Join(' ', css.TrimStart(), AdditionalAttributes.GetValue("link-class"));
        }
	}

	internal string IconCssClass => string.Join(' ', "material-icons", AdditionalAttributes.GetValue("icon-class"));
}
