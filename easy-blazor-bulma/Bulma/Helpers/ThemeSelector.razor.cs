using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace easy_blazor_bulma;

/// <summary>
/// Provides a button that toggles between the light and dark Bulma CSS themes.
/// </summary>
public partial class ThemeSelector : ComponentBase
{
	/// <summary>
	/// Applies the theme saved by the user on component initialization when true.
	/// </summary>
	[Parameter]
	public bool LoadUserPreference { get; set; } = true;

	/// <summary>
	/// The HTML id attribute assigned to the light Easy Blazor Bulma CSS stylesheet.
	/// </summary>
	[Parameter]
	public string LightThemeId { get; set; } = "easy-blazor-bulma";

	/// <summary>
	/// The HTML id attribute assigned to the dark Easy Blazor Bulma CSS stylesheet.
	/// </summary>
	[Parameter]
	public string DarkThemeId { get; set; } = "easy-blazor-bulma-dark";

	/// <summary>
	/// The keyname to store the user theme preference under.
	/// </summary>
	[Parameter]
	public string DarkModeKeyName { get; set; } = "easyblazorbulma.isdarkmode";

	/// <summary>
	/// Specifies whether the component will be contained in the navbar.
	/// </summary>
	[Parameter]
	public bool IsNavbarItem { get; set; } = true;

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private readonly string[] Filter = ["class"];

	[Inject]
	private IServiceProvider ServiceProvider { get; init; } = default!;

	private IJSRuntime? JsRuntime;

	private string MainCssClass
	{
		get
		{
			var css = "";

			if (IsNavbarItem)
				css += " navbar-item";

			return string.Join(' ', css, AdditionalAttributes.GetClass("class"));
		}
	}

	private bool IsDarkMode;
	private string Icon => IsDarkMode ? "dark_mode" : "light_mode";

	/// <inheritdoc />
	protected async override Task OnInitializedAsync()
	{
		// Get services
		JsRuntime = ServiceProvider.GetService<IJSRuntime>();

		if (JsRuntime == null)
			return;

		// Determine current mode
		var isOsDarkMode = await JsRuntime.IsOsDarkMode();

		if (LoadUserPreference == false)
		{
			IsDarkMode = isOsDarkMode;
			return;
		}

		var isUserDarkMode = await JsRuntime.ReadStorage(DarkModeKeyName);

		if (string.IsNullOrWhiteSpace(isUserDarkMode))
		{
			IsDarkMode = isOsDarkMode;
			return;
		}

		// Set mode
		var isDarkMode = isUserDarkMode.Equals("true", StringComparison.OrdinalIgnoreCase);

		if (isDarkMode != isOsDarkMode)
		{
			if (isOsDarkMode)
				await SetActiveTheme(LightThemeId, DarkThemeId);
			else
				await SetActiveTheme(DarkThemeId, LightThemeId);
		}

		IsDarkMode = isDarkMode;
	}

	private async Task ToggleMode()
	{
		if (IsDarkMode)
			await SetActiveTheme(LightThemeId, DarkThemeId);
		else
			await SetActiveTheme(DarkThemeId, LightThemeId);

		IsDarkMode = !IsDarkMode;

		if (JsRuntime != null)
			await JsRuntime.WriteStorage(DarkModeKeyName, IsDarkMode.ToString());
	}

	private async Task SetActiveTheme(string activate, string deactivate)
	{
		if (JsRuntime != null)
		{
			await JsRuntime.ToggleStyleSheet(activate, true, true);
			await JsRuntime.ToggleStyleSheet(deactivate, false);
		}
	}
}
