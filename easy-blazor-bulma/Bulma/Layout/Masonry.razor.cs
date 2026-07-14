using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using easy_core;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// Provides a Masonry.js-powered cascading grid layout container.
/// </summary>
/// <remarks>
/// Use with <see cref="MasonryItem"/> or any child elements matching <see cref="ItemSelector"/>.
/// Call <see cref="Layout"/> after image/content size changes for best results.
/// </remarks>
public partial class Masonry : ComponentBase, IAsyncDisposable
{
    /// <summary>
    /// Default column width in pixels when neither <see cref="ColumnWidth"/> nor <see cref="ColumnWidthSelector"/> is specified.
    /// </summary>
    private const int DefaultColumnWidth = 240;

	/// <summary>
	/// CSS selector used by Masonry.js to identify layout items.
	/// </summary>
	[Parameter]
	public string ItemSelector { get; set; } = MasonryItem.DefaultSelector;

    /// <summary>
    /// Fixed column width in pixels.
    /// </summary>
    /// <remarks>
    /// Use when columns are fixed-size in the Masonry layout. Don't combine with <see cref="ColumnWidthSelector"/>.
    /// </remarks>
    [Parameter]
	public int? ColumnWidth { get; set; }

    /// <summary>
    /// CSS selector for a sizing element (e.g. <c>.grid-sizer</c>) used by Masonry.js to calculate column width.
    /// </summary>
    /// <remarks>
    /// Element must be inside the Masonry container. Don't combine with <see cref="ColumnWidth"/>.
    /// </remarks>
    [Parameter]
	public string? ColumnWidthSelector { get; set; }

	/// <summary>
	/// Space between items in pixels.
	/// </summary>
	[Parameter]
	public int Gutter { get; set; }

    /// <summary>
    /// Whether to use percentage-based positioning.
    /// </summary>
	/// <remarks>
	/// Typically true for responsive layouts (recommended with <see cref="ColumnWidthSelector"/>).
	/// </remarks>
    [Parameter]
	public bool PercentPosition { get; set; } = true;

	/// <summary>
	/// Places items left-to-right when true.
	/// </summary>
	[Parameter]
	public bool HorizontalOrder { get; set; }

	/// <summary>
	/// CSS transition duration for layout changes.
	/// </summary>
	/// <remarks>
	/// Typically used to animate item repositioning when the layout changes.
	/// </remarks>
	[Parameter]
	public TimeSpan TransitionDuration { get; set; } = TimeSpan.FromMilliseconds(400);

	/// <summary>
	/// Automatically initializes Masonry on first render when true.
	/// </summary>
	[Parameter]
	public bool InitializeOnRender { get; set; } = true;

	/// <summary>
	/// Child content displayed inside the Masonry container.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Additional attributes applied to the root container.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Provides access to JavaScript interop functionality for initializing and managing the Masonry.js layout.
    /// </summary>
    [Inject]
	private IJSRuntime JsRuntime { get; init; } = default!;

    /// <summary>
    /// CSS classes to filter out from AdditionalAttributes to avoid duplication in the main container.
    /// </summary>
    private readonly string[] Filter = ["class"];

    /// <summary>
    /// Unique identifier for the Masonry container, generated to ensure no conflicts with other elements on the page.
    /// </summary>
    private readonly string GeneratedId = $"masonry-{Guid.NewGuid().ToHtmlId():N}";

    /// <summary>
    /// Indicates whether the Masonry layout has been initialized. Used to prevent redundant initialization and manage state.
    /// </summary>
    private bool IsInitialized;

    /// <summary>
    /// Stores the last known configuration fingerprint to detect changes in options.
    /// </summary>
    private string? LastOptionsFingerprint;

    /// <summary>
    /// Combines the base "masonry" class with any additional classes provided in AdditionalAttributes, ensuring proper styling and layout behavior.
    /// </summary>
    private string MainCssClass => string.Join(' ', "masonry", AdditionalAttributes.GetValue("class"));

    /// <summary>
    /// Gets the unique identifier for the Masonry container, which is used in JavaScript interop calls to reference the correct DOM element.
    /// </summary>
    private string ContainerId => GeneratedId;

	/// <inheritdoc />
	protected async override Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && InitializeOnRender)
			await Initialize();
	}

	/// <inheritdoc />
	protected async override Task OnParametersSetAsync()
	{
		if (IsInitialized == false)
			return;

		var fingerprint = BuildOptionsFingerprint();

        // If the fingerprint hasn't changed, no need to update options.
        if (string.Equals(LastOptionsFingerprint, fingerprint, StringComparison.Ordinal))
			return;

		await UpdateOptions();
	}

    /// <summary>
    /// Initializes the Masonry.js layout by loading the script if necessary and applying the configuration options.
    /// </summary>
    private async Task<bool> Initialize()
	{
		var scriptReady = await JsRuntime.LoadMasonryScriptIfMissing(TimeSpan.FromSeconds(1));

		if (scriptReady == false)
			return false;

		var options = BuildOptions();
		IsInitialized = await JsRuntime.MasonryInitialize(ContainerId, options);

		if (IsInitialized)
			LastOptionsFingerprint = BuildOptionsFingerprint();

		return IsInitialized;
	}

	/// <summary>
	/// Applies current option values to the existing Masonry instance.
	/// </summary>
	/// <param name="relayout">When true, recalculates item positions after options are applied.</param>
	/// <returns>True if the operation was successful; otherwise, false.</returns>
	public async Task<bool> UpdateOptions(bool relayout = true)
	{
		if (IsInitialized == false)
			return false;

		var options = BuildOptions();
		var updated = await JsRuntime.MasonryUpdateOptions(ContainerId, options, relayout);

		if (updated)
			LastOptionsFingerprint = BuildOptionsFingerprint();

		return updated;
	}

	/// <summary>
	/// Recalculates item positions.
	/// </summary>
	public async Task<bool> Layout()
	{
		if (IsInitialized == false)
			return false;

		return await JsRuntime.MasonryLayout(ContainerId);
	}

	/// <summary>
	/// Reloads items then re-reads item elements.
	/// </summary>
	public async Task<bool> ReloadItems()
	{
		if (IsInitialized == false)
			return false;

		return await JsRuntime.MasonryReloadItems(ContainerId);
	}

    /// <summary>
    /// Reloads items then re-reads item elements and recalculates item positions.
    /// </summary>
    /// <returns>True if the operation was successful; otherwise, false.</returns>
    public async Task<bool> Append()
	{
		if (IsInitialized == false)
			return false;

		var reloaded = await JsRuntime.MasonryReloadItems(ContainerId);

		if (reloaded == false)
			return false;

		return await JsRuntime.MasonryLayout(ContainerId);
	}

    /// <summary>
    /// Builds the options object to pass to Masonry.js based on the component parameters.
    /// </summary>
    /// <returns>A dictionary containing the Masonry.js options.</returns>
    private Dictionary<string, object?> BuildOptions()
	{
		object? columnWidth = DefaultColumnWidth;

        // Determine the column width based on the provided parameters.
        // If ColumnWidthSelector is specified, it takes precedence over ColumnWidth.
        if (string.IsNullOrWhiteSpace(ColumnWidthSelector) == false)
			columnWidth = ColumnWidthSelector;
		else if (ColumnWidth.HasValue)
			columnWidth = ColumnWidth.Value;

		return new Dictionary<string, object?>
		{
			["itemSelector"] = ItemSelector,
			["columnWidth"] = columnWidth,
			["gutter"] = Gutter,
			["percentPosition"] = PercentPosition,
			["horizontalOrder"] = HorizontalOrder,
			["transitionDuration"] = ToCssDuration(TransitionDuration)
		};
	}

    /// <summary>
    /// Converts a TimeSpan to a CSS-compatible duration string in milliseconds.
    /// </summary>
    /// <param name="duration">The duration to convert.</param>
    /// <returns>A CSS-compatible duration string in milliseconds.</returns>
    private static string ToCssDuration(TimeSpan duration)
	{
		return $"{duration.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture)}ms";
	}

    /// <summary>
    /// Builds a fingerprint string representing the current configuration options.
    /// </summary>
    /// <returns>A string representing the current configuration options.</returns>
    private string BuildOptionsFingerprint()
	{
        // Use ColumnWidthSelector if specified; otherwise, use ColumnWidth or the default value.
        var columnWidth = string.IsNullOrWhiteSpace(ColumnWidthSelector)
			? (ColumnWidth.HasValue ? ColumnWidth.Value.ToString(CultureInfo.InvariantCulture) : DefaultColumnWidth.ToString(CultureInfo.InvariantCulture))
			: $"selector:{ColumnWidthSelector}";

		return string.Join("|",
			ItemSelector,
			columnWidth,
			Gutter.ToString(CultureInfo.InvariantCulture),
			PercentPosition,
			HorizontalOrder,
			ToCssDuration(TransitionDuration));
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (IsInitialized)
			await JsRuntime.MasonryDestroy(ContainerId);

		GC.SuppressFinalize(this);
	}
}