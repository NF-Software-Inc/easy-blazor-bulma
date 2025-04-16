using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// A navigation menu to use at the top of the screen.
/// </summary>
/// <remarks>
/// There are 5 additional attributes that can be used: brand-class, burger-class, menu-class, a-class, and logo-class. Each of which apply CSS classes to the resulting elements as per their names.
/// <see href="https://bulma.io/documentation/components/navbar/">Bulma Documentation</see>
/// </remarks>
public partial class Navbar : ComponentBase
{
	/// <summary>
	/// The name to display in the top left of the navbar.
	/// </summary>
	[Parameter]
	public string? DisplayText { get; set; }

	/// <summary>
	/// An icon to display in the top left of the navbar.
	/// </summary>
	[Parameter]
    public string? LogoIcon { get; set; }

	/// <summary>
	/// The URL of an image to display in the top left of the navbar.
	/// </summary>
	[Parameter]
    public string? LogoUrl { get; set; }

	/// <summary>
	/// Specifies whether to display the burger icon for mobile devices.
	/// </summary>
	[Parameter]
	public bool UseBurger { get; set; } = true;

	/// <summary>
	/// The content to display on the left side of the navbar.
	/// </summary>
	[Parameter]
	public RenderFragment? NavbarStart { get; set; }

	/// <summary>
	/// The content to display on the right side of the navbar.
	/// </summary>
	[Parameter]
	public RenderFragment? NavbarEnd { get; set; }

	/// <summary>
	/// Fallback for content to display within the navbar. Applies when neither <see cref="NavbarStart"/> nor <see cref="NavbarEnd"/> are provided.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private readonly string[] Filter = new[] { "class", "id", "role", "aria-label", "href", "brand-class", "burger-class", "menu-class", "a-class", "logo-class", "img-class" };

	private bool IsActive;
	private string? Id;
	private string Href = "";

	private string MainCssClass => string.Join(' ', "navbar", AdditionalAttributes.GetClass("class"));

    private string BrandCssClass => string.Join(' ', "navbar-brand no-select", AdditionalAttributes.GetClass("brand-class"));

    private string BurgerCssClass
	{
		get
		{
			var css = "navbar-burger";

			if (IsActive)
				css += " is-active";

            return string.Join(' ', css, AdditionalAttributes.GetClass("burger-class"));
        }
	}

	private string MenuCssClass
	{
		get
		{
			var css = "navbar-menu is-unselectable";

			if (IsActive)
				css += " is-active";

            return string.Join(' ', css, AdditionalAttributes.GetClass("menu-class"));
        }
	}

	private string LinkCssClass => string.Join(' ', "navbar-item", AdditionalAttributes.GetClass("a-class"));

	private string LogoCssClass => AdditionalAttributes.GetClass("logo-class") ?? AdditionalAttributes.GetClass("img-class") ?? "mr-2";

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		if (string.IsNullOrWhiteSpace(Id))
			Id = AdditionalAttributes.GetValue("id") ?? Guid.NewGuid().ToString("N");

		if (string.IsNullOrWhiteSpace(Href))
			Href = AdditionalAttributes.GetValue("href") ?? string.Empty;
	}

	private void ToggleMenu()
	{
		IsActive = !IsActive;
	}
}
