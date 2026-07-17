using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using easy_core;
using System.Globalization;

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
	/// Use a value from 0 to 1.
	/// Lower values trigger earlier.
	/// More information on <see href="https://developer.mozilla.org/en-US/docs/Web/API/Intersection_Observer_API#threshold">Mozilla Developer Network</see>.
	/// </remarks>
	[Parameter]
	public double Threshold { get; set; } = 0.1;

	/// <summary>
	/// Margin around the viewport used by the intersection observer.
	/// </summary>
	/// <remarks>
	/// Accepts CSS margin syntax such as <c>"0px"</c> or <c>"200px 0px"</c>.
	/// Positive values trigger earlier; negative values trigger later.
	/// More information on <see href="https://developer.mozilla.org/en-US/docs/Web/API/Intersection_Observer_API#rootmargin">Mozilla Developer Network</see>.
	/// </remarks>
	[Parameter]
	public string RootMargin { get; set; } = "0px";

	/// <summary>
	/// Event callback indicating that more content should be loaded when the user scrolls to the sentinel.
	/// </summary>
	[EditorRequired]
	[Parameter]
	public required EventCallback OnLoadMore { get; set; }

	/// <summary>
	/// Event callback indicating that the load operation has completed.
	/// </summary>
	internal Func<Task>? OnLoadComplete { get; set; }

	/// <summary>
	/// Enables or disables infinite scroll behavior.
	/// </summary>
	[Parameter]
	public bool IsEnabled { get; set; } = true;

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
	private string? ObserverConfigFingerprint;

	private int LoadInProgress;

	/// <inheritdoc />
	protected async override Task OnAfterRenderAsync(bool firstRender)
	{
		DotNetReference ??= DotNetObjectReference.Create(new SentinelCallbackBridge(this));

		// Start, restart, or stop observing the sentinel
		var fingerprint = string.Join("|", Threshold.ToString("0.###############", CultureInfo.InvariantCulture), RootMargin);

		if (IsEnabled && IsObserving == false)
		{
			IsObserving = await JsRuntime.MasonryObserveInfiniteScroll(SentinelId, DotNetReference, Threshold, RootMargin);

			if (IsObserving)
				ObserverConfigFingerprint = fingerprint;
		}
		else if (IsEnabled && IsObserving && string.Equals(ObserverConfigFingerprint, fingerprint, StringComparison.Ordinal) == false)
		{
			await JsRuntime.MasonryUnobserveInfiniteScroll(SentinelId);

			IsObserving = await JsRuntime.MasonryObserveInfiniteScroll(SentinelId, DotNetReference, Threshold, RootMargin);
			ObserverConfigFingerprint = IsObserving ? fingerprint : null;
		}
		else if (IsEnabled == false && IsObserving)
		{
			await JsRuntime.MasonryUnobserveInfiniteScroll(SentinelId);

			IsObserving = false;
			ObserverConfigFingerprint = null;
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

		if (Interlocked.Exchange(ref LoadInProgress, 1) == 1)
			return;

		try
		{
			await OnLoadMore.InvokeAsync();

			if (OnLoadComplete != null)
				await OnLoadComplete.Invoke();
		}
		finally
		{
			Volatile.Write(ref LoadInProgress, 0);
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
		private readonly MasonryInfiniteScroll InfiniteScrollReference;

		public SentinelCallbackBridge(MasonryInfiniteScroll reference)
		{
			InfiniteScrollReference = reference;
		}

		/// <summary>
		/// JSInvokable method called when the sentinel becomes visible in the viewport.
		/// </summary>
		/// <returns>A task representing the asynchronous operation.</returns>
		[JSInvokable]
		public Task OnSentinelVisible() => InfiniteScrollReference.OnSentinelVisibleAsync();
	}
}
