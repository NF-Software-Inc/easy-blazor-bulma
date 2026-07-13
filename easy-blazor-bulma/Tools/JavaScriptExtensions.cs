using Microsoft.JSInterop;
using System.Text.Json;

namespace easy_blazor_bulma;

/// <summary>
/// Extension methods to simplify usage of the JavaScript functions from C#.
/// </summary>
public static class JavaScriptExtensions
{
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
}
