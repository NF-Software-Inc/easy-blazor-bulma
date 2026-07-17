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
	/// CSS selector used by Masonry.js to identify layout items.
	/// </summary>
	[Parameter]
	public string ItemSelector { get; set; } = ".masonry-item";

	/// <summary>
	/// Fixed column width in pixels.
	/// </summary>
	/// <remarks>
	/// Use when columns are fixed-size in the Masonry layout. Don't combine with <see cref="ColumnWidthSelector"/>.
	/// </remarks>
	[Parameter]
	public int? ColumnWidth { get; set; }

	/// <summary>
	/// CSS selector for a sizing element used by Masonry.js to calculate column width (e.g. <c>.grid-sizer</c>).
	/// </summary>
	/// <remarks>
	/// Placeholder element must be added inside the Masonry container with this class. Don't combine with <see cref="ColumnWidth"/>.
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

	[CascadingParameter]
	private MasonryInfiniteScroll? Parent { get; init; }

	/// <summary>
	/// Additional attributes applied to the root container.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	[Inject]
	private IJSRuntime JsRuntime { get; init; } = default!;

	private readonly string[] Filter = ["class"];
	private readonly string ContainerId = $"masonry-{Guid.NewGuid().ToHtmlId():N}";

	private bool IsInitialized;
	private bool PendingOptionsUpdate;
	private string? LastOptionsFingerprint;

	private readonly SemaphoreSlim LayoutGate = new(1, 1);
	private int PendingLayoutRequests;

	private string MainCssClass => string.Join(' ', "masonry", AdditionalAttributes.GetValue("class"));

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		if (Parent != null)
			Parent.OnLoadComplete += Refresh;
	}

	/// <inheritdoc />
	protected async override Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && InitializeOnRender)
			await Initialize();

		if (PendingOptionsUpdate && IsInitialized)
		{
			PendingOptionsUpdate = false;

			await UpdateOptions();
		}
	}

	/// <inheritdoc />
	protected override void OnParametersSet()
	{
		if (IsInitialized == false)
			return;

		if (string.Equals(LastOptionsFingerprint, BuildOptionsFingerprint(), StringComparison.Ordinal) == false)
			PendingOptionsUpdate = true;
	}

	/// <summary>
	/// Initializes the Masonry.js layout by loading the script if necessary and applying the configuration options.
	/// </summary>
	private async Task<bool> Initialize()
	{
		var ready = await JsRuntime.LoadScript("_content/Easy.Blazor.Bulma/js/masonry.pkgd.min.js", "Masonry", TimeSpan.FromSeconds(1));

		if (ready == false)
			return false;

		IsInitialized = await JsRuntime.MasonryInitialize(ContainerId, BuildOptions());

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

		var updated = await JsRuntime.MasonryUpdateOptions(ContainerId, BuildOptions(), relayout);

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

		Interlocked.Increment(ref PendingLayoutRequests);

		await LayoutGate.WaitAsync();

		try
		{
			while (Interlocked.Exchange(ref PendingLayoutRequests, 0) > 0)
			{
				var success = await JsRuntime.MasonryLayout(ContainerId);

				if (success == false)
					return false;
			}

			return true;
		}
		finally
		{
			LayoutGate.Release();
		}
	}

	/// <summary>
	/// Refreshes the layout and reloads items.
	/// </summary>
	public async Task<bool> Refresh()
	{
		if (IsInitialized == false)
			return false;

		Interlocked.Increment(ref PendingLayoutRequests);

		await LayoutGate.WaitAsync();

		try
		{
			while (Interlocked.Exchange(ref PendingLayoutRequests, 0) > 0)
			{
				var success = await JsRuntime.MasonryRefresh(ContainerId);

				if (success == false)
					return false;
			}

			return true;
		}
		finally
		{
			LayoutGate.Release();
		}
	}

	/// <summary>
	/// Reloads items then re-reads item elements.
	/// </summary>
	public async Task<bool> ReloadItems()
	{
		if (IsInitialized == false)
			return false;

		Interlocked.Increment(ref PendingLayoutRequests);

		await LayoutGate.WaitAsync();

		try
		{
			while (Interlocked.Exchange(ref PendingLayoutRequests, 0) > 0)
			{
				var success = await JsRuntime.MasonryReloadItems(ContainerId);

				if (success == false)
					return false;
			}

			return true;
		}
		finally
		{
			LayoutGate.Release();
		}
	}

	/// <summary>
	/// Builds the options object to pass to Masonry.js based on the component parameters.
	/// </summary>
	private Dictionary<string, object?> BuildOptions()
	{
		object? columnWidth;

		if (string.IsNullOrWhiteSpace(ColumnWidthSelector) == false)
			columnWidth = ColumnWidthSelector;
		else if (ColumnWidth.HasValue)
			columnWidth = ColumnWidth.Value;
		else
			columnWidth = 240;

		var options = new Dictionary<string, object?>
		{
			["itemSelector"] = ItemSelector,
			["columnWidth"] = columnWidth,
			["gutter"] = Gutter,
			["horizontalOrder"] = HorizontalOrder,
			["transitionDuration"] = TransitionDuration.TotalMilliseconds.ToString("0.###", CultureInfo.InvariantCulture) + "ms"
		};

		if (string.IsNullOrWhiteSpace(ColumnWidthSelector) == false)
			options["percentPosition"] = PercentPosition;

		return options;
	}

	/// <summary>
	/// Builds a fingerprint string representing the current configuration options.
	/// </summary>
	private string BuildOptionsFingerprint()
	{
		return string.Join("|", BuildOptions().Select(x => x.Value?.ToString()));
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (Parent != null)
			Parent.OnLoadComplete -= Refresh;

		if (IsInitialized)
			await JsRuntime.MasonryDestroy(ContainerId);

		LayoutGate.Dispose();
		GC.SuppressFinalize(this);
	}
}
