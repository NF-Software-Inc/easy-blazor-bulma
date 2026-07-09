using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

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
	/// CSS selector used by Masonry.js to identify layout items.
	/// </summary>
	[Parameter]
	public string ItemSelector { get; set; } = MasonryItem.DefaultSelector;

	/// <summary>
	/// Fixed column width in pixels.
	/// </summary>
	[Parameter]
	public int? ColumnWidth { get; set; }

	/// <summary>
	/// CSS selector for an element used to size columns.
	/// </summary>
	[Parameter]
	public string? ColumnWidthSelector { get; set; }

	/// <summary>
	/// Space between items in pixels.
	/// </summary>
	[Parameter]
	public int Gutter { get; set; }

	/// <summary>
	/// Uses percentage-based positioning when true.
	/// </summary>
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
	[Parameter]
	public string TransitionDuration { get; set; } = "0.4s";

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
    /// Service provider for resolving dependencies. Used to obtain an <see cref="IJSRuntime"/> instance for JavaScript interop.
    /// </summary>
    [Inject]
	private IServiceProvider ServiceProvider { get; init; } = default!;

	private readonly string[] Filter = ["class", "id"];
	private readonly string GeneratedId = $"masonry-{Guid.NewGuid():N}";
	private IJSRuntime? JsRuntime;
	private bool IsInitialized;

	private string MainCssClass => string.Join(' ', "masonry", AdditionalAttributes.GetValue("class"));

	private string ContainerId => AdditionalAttributes.GetValue("id") ?? GeneratedId;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		JsRuntime = ServiceProvider.GetService<IJSRuntime>();
	}

	/// <inheritdoc />
	protected async override Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && InitializeOnRender)
			await Initialize();
	}

	/// <summary>
	/// Initializes Masonry.js on this container.
	/// </summary>
	public async Task<bool> Initialize()
	{
		if (JsRuntime == null)
			return false;

		var options = BuildOptions();
		IsInitialized = await JsRuntime.MasonryInitialize(ContainerId, options);
		return IsInitialized;
	}

	/// <summary>
	/// Recalculates item positions.
	/// </summary>
	public async Task<bool> Layout()
	{
		if (JsRuntime == null || IsInitialized == false)
			return false;

		return await JsRuntime.MasonryLayout(ContainerId);
	}

	/// <summary>
	/// Reloads items then re-reads item elements.
	/// </summary>
	public async Task<bool> ReloadItems()
	{
		if (JsRuntime == null || IsInitialized == false)
			return false;

		return await JsRuntime.MasonryReloadItems(ContainerId);
	}

    /// <summary>
    /// Reloads items then re-reads item elements and recalculates item positions.
    /// </summary>
    /// <returns>True if the operation was successful; otherwise, false.</returns>
    public async Task<bool> Append()
	{
		if (JsRuntime == null || IsInitialized == false)
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
		object? columnWidth = null;

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
			["transitionDuration"] = TransitionDuration
		};
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (JsRuntime != null && IsInitialized)
			await JsRuntime.MasonryDestroy(ContainerId);

		GC.SuppressFinalize(this);
	}
}