using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using easy_core;

namespace easy_blazor_bulma;

/// <summary>
/// Provides optional infinite-scroll behavior using an IntersectionObserver sentinel.
/// </summary>
public partial class MasonryInfiniteScroll : ComponentBase, IAsyncDisposable
{
	/// <summary>
	/// Intersection ratio that triggers <see cref="OnLoadMore"/>.
	/// </summary>
	/// <remarks>
	/// Use a value from 0 to 1. Lower values trigger earlier.
	/// </remarks>
	[Parameter]
	public double Threshold { get; set; } = 0.1;

	/// <summary>
	/// Margin around the viewport used by the intersection observer.
	/// </summary>
	/// <remarks>
	/// Accepts CSS margin syntax such as <c>"0px"</c> or <c>"200px 0px"</c>.
	/// Positive values trigger earlier; negative values trigger later.
	/// </remarks>
	[Parameter]
	public string RootMargin { get; set; } = "0px";

	/// <summary>
	/// Event raised when the sentinel enters the viewport.
	/// </summary>
	[Parameter]
	public EventCallback OnLoadMore { get; set; }

	/// <summary>
	/// Enables or disables infinite scroll behavior.
	/// </summary>
	[Parameter]
	public bool IsEnabled { get; set; } = true;

	/// <summary>
	/// Prevents new load requests while true.
	/// </summary>
	[Parameter]
	public bool IsBusy { get; set; }

	/// <summary>
	/// Content to render before the sentinel.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Inject]
	private IJSRuntime JsRuntime { get; init; } = default!;

	private readonly string SentinelId = $"masonry-infinite-scroll-{Guid.NewGuid().ToHtmlId():N}";
	private DotNetObjectReference<SentinelCallbackBridge>? DotNetReference;
	private bool IsObserving;
    /// <summary>
    /// Flag to indicate whether a load operation is currently in progress. This prevents multiple concurrent load requests when the sentinel becomes visible.
    /// </summary>
    private int _loadInProgress;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		DotNetReference = DotNetObjectReference.Create(new SentinelCallbackBridge(this));
	}

	/// <inheritdoc />
	protected async override Task OnAfterRenderAsync(bool firstRender)
	{
		if (DotNetReference == null)
			return;

		if (IsEnabled && IsObserving == false)
		{
			IsObserving = await JsRuntime.MasonryObserveInfiniteScroll(
				SentinelId,
				DotNetReference,
				Threshold,
				RootMargin);
		}
		else if (IsEnabled == false && IsObserving)
		{
			await JsRuntime.MasonryUnobserveInfiniteScroll(SentinelId);
			IsObserving = false;
		}
	}

    /// <summary>
    /// Called when the sentinel becomes visible in the viewport.
    /// This method invokes the <see cref="OnLoadMore"/> callback if infinite scrolling is enabled and no load operation is currently in progress.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    internal async Task OnSentinelVisibleAsync()
	{
        if (IsEnabled == false || OnLoadMore.HasDelegate == false)
            return;

        // Set a flag indicating that a load is in progress.
        if (Interlocked.Exchange(ref _loadInProgress, 1) == 1)
            return;

        try
        {
            if (IsBusy)
                return;

            await OnLoadMore.InvokeAsync();
        }
        finally
        {
            // Reset the load-in-progress flag to allow future load requests.
            Volatile.Write(ref _loadInProgress, 0);
        }
    }

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (IsObserving)
			await JsRuntime.MasonryUnobserveInfiniteScroll(SentinelId);

		DotNetReference?.Dispose();
		GC.SuppressFinalize(this);
	}


    /// <summary>
    /// A bridge class to handle JS callbacks for the sentinel visibility.
    /// </summary>
    /// <remarks>
    /// This class is used to decouple the JS callback from the main component, allowing for better encapsulation and easier testing.
    /// </remarks>
    internal sealed class SentinelCallbackBridge
    {
        /// <summary>
        /// Reference to the owning <see cref="MasonryInfiniteScroll"/> component.
        /// </summary>
        private readonly MasonryInfiniteScroll _owner;

        public SentinelCallbackBridge(MasonryInfiniteScroll owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// JSInvokable method called when the sentinel becomes visible in the viewport.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [JSInvokable]
        public Task OnSentinelVisible()
            => _owner.OnSentinelVisibleAsync();
    }
}