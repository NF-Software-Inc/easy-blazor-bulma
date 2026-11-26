using easy_core;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace easy_blazor_bulma;

/// <summary>
/// A reusable floating button that appears after scrolling past a threshold and scrolls back to top when clicked.
/// </summary>
public sealed partial class ScrollToTopButton : EasyComponentBase, IAsyncDisposable
{
	/// <summary>
	/// The scroll position threshold in pixels required before the button is displayed.
	/// </summary>
	[Parameter]
	public int VisibilityThreshold { get; set; } = 250;

	/// <summary>
	/// The icon name to display on the button.
	/// </summary>
	[Parameter]
	public string Icon { get; set; } = "keyboard_double_arrow_up";

	/// <summary>
	/// Fires after the button scrolls the page to top.
	/// </summary>
	[Parameter]
	public EventCallback ScrolledToTop { get; init; }

	/// <summary>
	/// Any additional attributes applied directly to the button.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	[Inject]
	private IJSRuntime JsRuntime { get; init; } = default!;

	/// <summary>
	/// A logical key passed to JS for listener registration/unregistration
	/// </summary>
	private readonly string ListenerId = Guid.NewGuid().ToHtmlId().ToString("N");

	/// <summary>
	/// Indicates whether the button is currently visible based on the scroll position.
	/// </summary>
	private bool IsVisible;

	private Interop? JavaScriptInterop;

	private readonly string[] Filter = ["class", "style"];

	private string ButtonCssClass
	{
		get
		{
			var css = "button is-link";

			if (IsVisible == false)
				css += " is-hidden";

			return string.Join(" ", css, AdditionalAttributes.GetValue("class"));
		}
	}

	private string ButtonCssStyle
	{
		get
		{
			var css = "";
			var custom = AdditionalAttributes.GetValue("style");

			if (custom == null || custom.Contains("position:") == false)
				css += " position: fixed;";

			if (custom == null || custom.Contains("bottom:") == false)
				css += " bottom: 0;";

			if (custom == null || custom.Contains("right:") == false)
				css += " right: 0;";

			if (custom == null || custom.Contains("z-index:") == false)
				css += " z-index: 30;";

			return string.Join(" ", css, custom);
		}
	}

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			JavaScriptInterop = new(this);
			await JsRuntime.RegisterScrollListener(ListenerId, JavaScriptInterop.ObjectReference, nameof(Interop.OnScrollVisibilityChanged), Math.Max(0, VisibilityThreshold));
		}

		await base.OnAfterRenderAsync(firstRender);
	}

	/// <summary>
	/// Scrolls the page to the top when the button is clicked.
	/// </summary>
	/// <returns>A task that represents the asynchronous operation.</returns>
	private async Task OnScrollToTop()
	{
		await JsRuntime.SetScroll(0, 0);

		if (ScrolledToTop.HasDelegate)
			await ScrolledToTop.InvokeAsync();
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		await JsRuntime.UnregisterScrollListener(ListenerId);

		JavaScriptInterop?.Dispose();
	}

	internal class Interop : IDisposable
	{
		internal ScrollToTopButton ButtonReference;
		internal readonly DotNetObjectReference<Interop> ObjectReference;

		internal Interop(ScrollToTopButton reference)
		{
			ButtonReference = reference;
			ObjectReference = DotNetObjectReference.Create(this);
		}

		/// <summary>
		/// Updates button visibility from JavaScript scroll events.
		/// </summary>
		[JSInvokable]
		public async Task OnScrollVisibilityChanged(bool isVisible)
		{
			if (ButtonReference.IsVisible == isVisible)
				return;

			ButtonReference.IsVisible = isVisible;

			await ButtonReference.StateHasChangedAsync();
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			ObjectReference.Dispose();
		}
	}
}
