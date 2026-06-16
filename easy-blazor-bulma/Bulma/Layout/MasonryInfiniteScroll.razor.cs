using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace easy_blazor_bulma;

/// <summary>
/// Provides optional infinite-scroll behavior using an IntersectionObserver sentinel.
/// </summary>
public partial class MasonryInfiniteScroll : ComponentBase, IAsyncDisposable
{
	/// <summary>
	/// Intersection ratio required to trigger <see cref="OnLoadMore"/>.
	/// </summary>
	[Parameter]
	public double Threshold { get; set; } = 0.1;

	/// <summary>
	/// Margin around the observer root.
	/// </summary>
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

    /// <summary>
    /// Service provider for resolving JS runtime. This is injected to avoid direct dependency on IJSRuntime.
    /// </summary>
    [Inject]
	private IServiceProvider ServiceProvider { get; init; } = default!;

	private readonly string SentinelId = $"masonry-infinite-scroll-{Guid.NewGuid():N}";
	private IJSRuntime? JsRuntime;
	private DotNetObjectReference<MasonryInfiniteScroll>? DotNetReference;
	private bool IsObserving;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		JsRuntime = ServiceProvider.GetService<IJSRuntime>();
		DotNetReference = DotNetObjectReference.Create(this);
	}

	/// <inheritdoc />
	protected async override Task OnAfterRenderAsync(bool firstRender)
	{
		if (JsRuntime == null || DotNetReference == null)
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
	/// JS callback invoked when the sentinel becomes visible.
	/// </summary>
	[JSInvokable]
	public async Task OnSentinelVisible()
	{
		if (IsEnabled == false || IsBusy || OnLoadMore.HasDelegate == false)
			return;

		await OnLoadMore.InvokeAsync();
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (JsRuntime != null && IsObserving)
			await JsRuntime.MasonryUnobserveInfiniteScroll(SentinelId);

		DotNetReference?.Dispose();
		GC.SuppressFinalize(this);
	}
}