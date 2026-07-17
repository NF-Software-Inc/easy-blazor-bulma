using Microsoft.JSInterop;
using System.Text.Json;

namespace easy_blazor_bulma;

/// <summary>
/// Extension methods to simplify usage of the JavaScript functions from C#.
/// </summary>
public static class JavaScriptExtensions
{
	#region Public functions

	/// <summary>
	/// Applies a style to the specified element by id.
	/// </summary>
	/// <param name="id">The id of the element to apply the style to.</param>
	/// <param name="style">The name of the style to apply.</param>
	/// <param name="value">The value to assign to the style.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> ApplyStyle(this IJSRuntime jSRuntime, string id, string style, string value, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.ApplyStyle", token ?? CancellationToken.None, id, style, value);
	}

	/// <summary>
	/// Navigates to the previous URL.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task Back(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.Back", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Navigates to the next URL.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task Forward(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.Forward", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Tests to see whether local storage is available in the current context.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> HasStorage(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.HasStorage", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Checks to see whether the operating system is using a dark mode theme.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> IsOsDarkMode(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.IsOsDarkMode", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Loads a JavaScript file and appends to the end of document.body.
	/// </summary>
	/// <param name="uri">The URI of the script to load.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> LoadScript(this IJSRuntime jSRuntime, string uri, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.LoadScript", token ?? CancellationToken.None, uri);
	}

	/// <summary>
	/// Loads a JavaScript file and appends to the end of document.body.
	/// </summary>
	/// <param name="uri">The URI of the script to load.</param>
	/// <param name="type">The name of a type contained in the script to test for after loading completes.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> LoadScript(this IJSRuntime jSRuntime, string uri, string type, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.LoadScript", token ?? CancellationToken.None, uri, type);
	}

	/// <summary>
	/// Loads a JavaScript file and appends to the end of document.body.
	/// </summary>
	/// <param name="uri">The URI of the script to load.</param>
	/// <param name="type">The name of a type contained in the script to test for after loading completes.</param>
	/// <param name="timeout">The duration to wait for the type to be available in the browser window.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> LoadScript(this IJSRuntime jSRuntime, string uri, string type, TimeSpan timeout, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.LoadScript", token ?? CancellationToken.None, uri, type, (int)timeout.TotalMilliseconds);
	}

	/// <summary>
	/// Checks for and returns the local storage item with the specified name.
	/// </summary>
	/// <param name="name">The name of the storage item to return.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<string?> ReadStorage(this IJSRuntime jSRuntime, string name, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<string?>("easyBlazorBulma.ReadStorage", token ?? CancellationToken.None, name);
	}

	/// <summary>
	/// Checks for and returns the local storage item with the specified name.
	/// </summary>
	/// <param name="name">The name of the storage item to return.</param>
	/// <param name="options">Options to pass to the JSON serializer.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<TValue?> ReadStorage<TValue>(this IJSRuntime jSRuntime, string name, JsonSerializerOptions? options, CancellationToken? token = null)
	{
		var value = await jSRuntime.InvokeAsync<string?>("easyBlazorBulma.ReadStorage", token ?? CancellationToken.None, name);

		if (string.IsNullOrWhiteSpace(value))
			return default;

		return JsonSerializer.Deserialize<TValue>(value, options);
	}

	/// <summary>
	/// Enables or disables the stylesheet with the provided identity value if found.
	/// </summary>
	/// <param name="id">The identity of the DOM element representing the stylesheet to update.</param>
	/// <param name="enable">Specifies which action to take. True to enable, false to disable.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> ToggleStyleSheet(this IJSRuntime jSRuntime, string id, bool enable, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.ToggleStyleSheet", token ?? CancellationToken.None, id, enable, false);
	}

	/// <summary>
	/// Enables or disables the stylesheet with the provided identity value if found.
	/// </summary>
	/// <param name="id">The identity of the DOM element representing the stylesheet to update.</param>
	/// <param name="enable">Specifies which action to take. True to enable, false to disable.</param>
	/// <param name="clearMediaText">Specifies whether to clear the media property.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> ToggleStyleSheet(this IJSRuntime jSRuntime, string id, bool enable, bool clearMediaText, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.ToggleStyleSheet", token ?? CancellationToken.None, id, enable, clearMediaText);
	}

	/// <summary>
	/// Creates or updates a local storage item with the provided values.
	/// </summary>
	/// <param name="name">The text identifier for the storage item.</param>
	/// <param name="value">The serialized value to store in the item.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> WriteStorage(this IJSRuntime jSRuntime, string name, string value, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.WriteStorage", token ?? CancellationToken.None, name, value);
	}

	/// <summary>
	/// Creates or updates a local storage item with the provided values.
	/// </summary>
	/// <param name="name">The text identifier for the storage item.</param>
	/// <param name="value">The value to store in the item.</param>
	/// <param name="options">Options to pass to the JSON serializer.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> WriteStorage<TValue>(this IJSRuntime jSRuntime, string name, TValue value, JsonSerializerOptions? options, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.WriteStorage", token ?? CancellationToken.None, name, JsonSerializer.Serialize(value, options));
	}

	#endregion

	#region Internal Masonry Interop

	/// <summary>
	/// Destroys a Masonry instance for the specified element id.
	/// </summary>
	/// <param name="id">The id of the Masonry container element.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	internal async static Task<bool> MasonryDestroy(this IJSRuntime jSRuntime, string id, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.Masonry.Destroy", token ?? CancellationToken.None, id);
	}

	/// <summary>
	/// Initializes a Masonry instance for the specified element id.
	/// </summary>
	/// <param name="id">The id of the element to initialize Masonry on.</param>
	/// <param name="options">The Masonry options object. Default is null.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	internal async static Task<bool> MasonryInitialize(this IJSRuntime jSRuntime, string id, object? options = null, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.Masonry.Initialize", token ?? CancellationToken.None, id, options);
	}

	/// <summary>
	/// Triggers a layout update for the Masonry instance.
	/// </summary>
	/// <param name="id">The id of the Masonry container element.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	internal async static Task<bool> MasonryLayout(this IJSRuntime jSRuntime, string id, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.Masonry.Layout", token ?? CancellationToken.None, id);
	}

	/// <summary>
	/// Starts observing a sentinel element for infinite scroll callbacks.
	/// </summary>
	/// <param name="sentinelId">The id of the sentinel element to observe.</param>
	/// <param name="dotNetReference">A DotNetObjectReference used for JS-to-.NET callback.</param>
	/// <param name="threshold">Intersection ratio required to trigger callback. Default is 0.1.</param>
	/// <param name="rootMargin">Margin around the root used for intersection checks. Default is "0px".</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	internal async static Task<bool> MasonryObserveInfiniteScroll<TComponent>(this IJSRuntime jSRuntime, string sentinelId, DotNetObjectReference<TComponent> dotNetReference, double threshold = 0.1, string rootMargin = "0px", CancellationToken? token = null) where TComponent : class
	{
		return await jSRuntime.InvokeAsync<bool>(
			"easyBlazorBulma.Masonry.ObserveInfiniteScroll",
			token ?? CancellationToken.None,
			sentinelId,
			dotNetReference,
			threshold,
			rootMargin
		);
	}

	/// <summary>
	/// Reloads items for the Masonry instance.
	/// </summary>
	/// <param name="id">The id of the Masonry container element.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	internal async static Task<bool> MasonryRefresh(this IJSRuntime jSRuntime, string id, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.Masonry.Refresh", token ?? CancellationToken.None, id);
	}

	/// <summary>
	/// Reloads items for the Masonry instance.
	/// </summary>
	/// <param name="id">The id of the Masonry container element.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	internal async static Task<bool> MasonryReloadItems(this IJSRuntime jSRuntime, string id, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.Masonry.ReloadItems", token ?? CancellationToken.None, id);
	}

	/// <summary>
	/// Stops observing a sentinel element for infinite scroll callbacks.
	/// </summary>
	/// <param name="sentinelId">The id of the sentinel element to stop observing.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	internal async static Task<bool> MasonryUnobserveInfiniteScroll(this IJSRuntime jSRuntime, string sentinelId, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.Masonry.UnobserveInfiniteScroll", token ?? CancellationToken.None, sentinelId);
	}

	/// <summary>
	/// Updates options for an existing Masonry instance.
	/// </summary>
	/// <param name="id">The id of the Masonry container element.</param>
	/// <param name="options">The options object to apply.</param>
	/// <param name="relayout">When true, triggers layout after applying options.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	internal async static Task<bool> MasonryUpdateOptions(this IJSRuntime jSRuntime, string id, object? options = null, bool relayout = true, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.Masonry.UpdateOptions", token ?? CancellationToken.None, id, options, relayout);
	}

	#endregion
}
