using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// A dropdown to use in a Navbar menu to contain additional items.
/// </summary>
/// <remarks>
/// There are 2 additional attributes that can be used: dropdown-class and link-class. They will apply CSS classes to the resulting elements as per their names.
/// <see href="https://bulma.io/documentation/components/navbar/">Bulma Documentation</see>
/// </remarks>
public partial class NavbarDropdown : ComponentBase
{
	/// <summary>
	/// The name to display on the navbar header item.
	/// </summary>
	[Parameter]
	public string? DisplayText { get; set; }

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

	/// <summary>
	/// Specifies whether the dropdown should use the full screen width.
	/// </summary>
	[Parameter]
	public bool IsFullWidth { get; set; }

	/// <summary>
	/// The content to display within the dropdown.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private readonly string[] Filter = new[] { "class", "dropdown-class", "link-class" };

    private bool IsActive;

	private string MainCssClass
	{
		get
		{
			var css = "navbar-item has-dropdown is-hoverable";

			if (IsFullWidth)
				css += " is-mega";

            return string.Join(' ', css, AdditionalAttributes.GetValue("class"));
        }
	}

	private string LinkCssClass
	{
		get
		{
			var css = "";

			if (string.IsNullOrWhiteSpace(Icon) == false)
				css += " ml-2";

			if (CompactDisplay && string.IsNullOrWhiteSpace(DisplayText) == false)
				css += " is-hidden-touch is-hidden-desktop-only is-hidden-widescreen-only";

            return string.Join(' ', css.TrimStart(), AdditionalAttributes.GetValue("link-class"));
        }
	}

	private string DropdownCssClass
	{
		get
		{
			var css = "navbar-dropdown";

			if (IsActive)
				css += " is-active";

            return string.Join(' ', css, AdditionalAttributes.GetValue("dropdown-class"));
        }
	}

	private void ToggleMenu()
	{
		IsActive = !IsActive;
	}
}
