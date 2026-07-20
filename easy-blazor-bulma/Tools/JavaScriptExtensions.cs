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
	/// Adds a new URI parameter.
	/// </summary>
	/// <param name="name">The name of the parameter to add.</param>
	/// <param name="value">The value to assign to the parameter.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task AddUriParameter(this IJSRuntime jSRuntime, string name, string? value, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.AddUriParameter", token ?? CancellationToken.None, name, value);
	}

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
	/// Checks for and requests permission to access the system camera.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> CheckCameraPermission(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.CheckCameraPermission", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Checks for and requests permission to display desktop notifications.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> CheckNotificationPermission(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.CheckNotificationPermission", token ?? CancellationToken.None, false);
	}

	/// <summary>
	/// Checks for and requests permission to display desktop notifications.
	/// </summary>
	/// <param name="reset">Specifies whether to re-request when already denied.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> CheckNotificationPermission(this IJSRuntime jSRuntime, bool reset, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.CheckNotificationPermission", token ?? CancellationToken.None, reset);
	}

	/// <summary>
	/// Checks for and requests permission to display desktop notifications.
	/// </summary>
	/// <param name="image">The base-64 image data to compress and resize.</param>
	/// <param name="width">The maximum width to resize the image to.</param>
	/// <param name="height">The maximum height to resize the image to.</param>
	/// <param name="quality">The quality to compress the image to (should be a value between 0 and 1).</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> CompressAndResize(this IJSRuntime jSRuntime, string image, int width, int height, float quality, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.CompressAndResize", token ?? CancellationToken.None, image, width, height, quality);
	}

	/// <summary>
	/// Checks for and removes the specified cache if it exists.
	/// </summary>
	/// <param name="name">The name of the cache to remove.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> DeleteCache(this IJSRuntime jSRuntime, string name, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.DeleteCache", token ?? CancellationToken.None, name);
	}

	/// <summary>
	/// Removes a cookie with the provided name.
	/// </summary>
	/// <param name="name">The text identifier for the cookie.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> DeleteCookie(this IJSRuntime jSRuntime, string name, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.WriteCookie", token ?? CancellationToken.None, name, null, 0);
	}

	/// <summary>
	/// Checks for and removes the local storage item with the specified name.
	/// </summary>
	/// <param name="name">The name of the storage item to remove.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> DeleteStorage(this IJSRuntime jSRuntime, string name, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.DeleteStorage", token ?? CancellationToken.None, name);
	}

	/// <summary>
	/// Displays a notification to the user.
	/// </summary>
	/// <param name="title">The value to set as the notification header.</param>
	/// <param name="message">The value to set as the notification text.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task DisplayNotification(this IJSRuntime jSRuntime, string title, string message, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.DisplayNotification", token ?? CancellationToken.None, title, message);
	}

	/// <summary>
	/// Displays a notification to the user.
	/// </summary>
	/// <param name="title">The value to set as the notification header.</param>
	/// <param name="message">The value to set as the notification text.</param>
	/// <param name="url">The URL to open when a notification is clicked.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task DisplayNotification(this IJSRuntime jSRuntime, string title, string message, string url, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.DisplayNotification", token ?? CancellationToken.None, title, message, url);
	}

	/// <summary>
	/// Converts the provided stream to a blob in memory and downloads the file.
	/// </summary>
	/// <param name="name">The name to assign the downloaded file.</param>
	/// <param name="reference">A reference to the stream to download.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task DownloadFile(this IJSRuntime jSRuntime, string name, DotNetStreamReference reference, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.DownloadFileFromStream", token ?? CancellationToken.None, name, reference);
	}

	/// <summary>
	/// Converts the provided byte array to a base-64 string and downloads the file.
	/// </summary>
	/// <param name="name">The name to assign the downloaded file.</param>
	/// <param name="bytes">An array of bytes containing the file data.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task DownloadFile(this IJSRuntime jSRuntime, string name, byte[] bytes, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.DownloadFileFromBase64", token ?? CancellationToken.None, name, Convert.ToBase64String(bytes));
	}

	/// <summary>
	/// Downloads the file at the specified URL.
	/// </summary>
	/// <param name="name">The name to assign the downloaded file.</param>
	/// <param name="url">The URL to download a file from.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task DownloadFile(this IJSRuntime jSRuntime, string name, string url, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.DownloadFileFromUrl", token ?? CancellationToken.None, name, url);
	}

	/// <summary>
	/// Decrypts the provided text using AES-GCM.
	/// </summary>
	/// <param name="text">The base-64 payload to decrypt.</param>
	/// <param name="password">The secret to decrypt the payload data with.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<string> Decrypt(this IJSRuntime jSRuntime, string text, string password, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<string>("easyBlazorBulma.Decrypt", token ?? CancellationToken.None, text, password);
	}

	/// <summary>
	/// Encrypts the provided text using AES-GCM.
	/// </summary>
	/// <param name="text">The plain text data to encrypt.</param>
	/// <param name="password">The secret to encrypt the payload data with.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<string> Encrypt(this IJSRuntime jSRuntime, string text, string password, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<string>("easyBlazorBulma.Encrypt", token ?? CancellationToken.None, text, password);
	}

	/// <summary>
	/// Moves the focussed element to the specified id tag or first element with the autofocus attribute.
	/// </summary>
	/// <param name="id">The identity tag of the element to focus.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> FocusElement(this IJSRuntime jSRuntime, string id, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.FocusElement", token ?? CancellationToken.None, id);
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
	/// Gets the current URI parameters.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<string?> GetUriParameters(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<string?>("easyBlazorBulma.GetUriParameters", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Returns the number of bytes remaining for IndexedDB storage.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<long> GetStorageBytes(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<long>("easyBlazorBulma.GetStorageBytes", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Returns a list of browser application caches.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<string[]> GetCaches(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<string[]>("easyBlazorBulma.GetCaches", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Returns the x and y scroll positions the browser currently is at.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<(int x, int y)> GetScroll(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		var coords = await jSRuntime.InvokeAsync<int[]>("easyBlazorBulma.GetScroll", token ?? CancellationToken.None);
		return (coords[0], coords[1]);
	}

	/// <summary>
	/// Returns a list of keys that are currently in the local storage.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<string[]> GetStorageKeys(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<string[]>("easyBlazorBulma.GetStorageKeys", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Returns the current height of the screen in pixels.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<int> GetWindowHeight(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<int>("easyBlazorBulma.GetWindowHeight", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Gets the current page title.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<string?> GetWindowTitle(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<string?>("easyBlazorBulma.GetWindowTitle", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Returns the current width of the screen in pixels.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<int> GetWindowWidth(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<int>("easyBlazorBulma.GetWindowWidth", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Tests to see whether desktop notifications are available in the current context.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> HasCamera(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.HasCamera", token ?? CancellationToken.None);
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
	/// Tests to see whether IndexedDB is available in the current context.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> HasIndexedDb(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.HasIndexedDb", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Tests to see whether desktop notifications are available in the current context.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> HasNotification(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.HasNotification", token ?? CancellationToken.None);
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
	/// Imports a JavaScript file.
	/// </summary>
	/// <param name="uri">The URI of the script to load.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public static async Task<IJSObjectReference?> ImportScript(this IJSRuntime jsRuntime, string uri, CancellationToken? token = null)
	{
		return await jsRuntime.InvokeAsync<IJSObjectReference?>("import", token ?? CancellationToken.None, uri);
	}

	/// <summary>
	/// Accepts a content stream and loads it as a blob into the element with the specified id value.
	/// </summary>
	/// <param name="id">The id of the element to load the data into.</param>
	/// <param name="reference">A reference to the stream containing the data.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task LoadBlob(this IJSRuntime jSRuntime, string id, DotNetStreamReference reference, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.LoadBlob", token ?? CancellationToken.None, id, null, reference);
	}

	/// <summary>
	/// Accepts a content stream and loads it as a blob into the element with the specified id value.
	/// </summary>
	/// <param name="id">The id of the element to load the data into.</param>
	/// <param name="mime">The MIME type of the content to load.</param>
	/// <param name="reference">A reference to the stream containing the data.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task LoadBlob(this IJSRuntime jSRuntime, string id, string? mime, DotNetStreamReference reference, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.LoadBlob", token ?? CancellationToken.None, id, mime, reference);
	}

	/// <summary>
	/// Accepts a byte array and loads it as a blob into the element with the specified id value.
	/// </summary>
	/// <param name="id">The id of the element to load the data into.</param>
	/// <param name="data">The array containing the file data.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task LoadBlob(this IJSRuntime jSRuntime, string id, MemoryStream stream, CancellationToken? token = null)
	{
		using var reference = new DotNetStreamReference(stream);

		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.LoadBlob", token ?? CancellationToken.None, id, null, reference);
	}

	/// <summary>
	/// Accepts a byte array and loads it as a blob into the element with the specified id value.
	/// </summary>
	/// <param name="id">The id of the element to load the data into.</param>
	/// <param name="mime">The MIME type of the content to load.</param>
	/// <param name="data">The array containing the file data.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task LoadBlob(this IJSRuntime jSRuntime, string id, string? mime, MemoryStream stream, CancellationToken? token = null)
	{
		using var reference = new DotNetStreamReference(stream);

		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.LoadBlob", token ?? CancellationToken.None, id, mime, reference);
	}

	/// <summary>
	/// Accepts a byte array and loads it as a blob into the element with the specified id value.
	/// </summary>
	/// <param name="id">The id of the element to load the data into.</param>
	/// <param name="data">The array containing the file data.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task LoadBlob(this IJSRuntime jSRuntime, string id, byte[] data, CancellationToken? token = null)
	{
		using var stream = new MemoryStream(data);
		using var reference = new DotNetStreamReference(stream);

		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.LoadBlob", token ?? CancellationToken.None, id, null, reference);
	}

	/// <summary>
	/// Accepts a byte array and loads it as a blob into the element with the specified id value.
	/// </summary>
	/// <param name="id">The id of the element to load the data into.</param>
	/// <param name="mime">The MIME type of the content to load.</param>
	/// <param name="data">The array containing the file data.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task LoadBlob(this IJSRuntime jSRuntime, string id, string? mime, byte[] data, CancellationToken? token = null)
	{
		using var stream = new MemoryStream(data);
		using var reference = new DotNetStreamReference(stream);

		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.LoadBlob", token ?? CancellationToken.None, id, mime, reference);
	}

	/// <summary>
	/// Loads the specified image from the database.
	/// </summary>
	/// <param name="name">The database to read from.</param>
	/// <param name="store">The table in the database to read from.</param>
	/// <param name="version">The version number of the database.</param>
	/// <param name="id">The key the image is stored under.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task LoadBlobFromIndexedDb(this IJSRuntime jSRuntime, string name, string store, int version, string id, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.LoadBlobFromIndexedDb", token ?? CancellationToken.None, name, store, version, id);
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
	/// Displays the print preview dialog.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task PrintPreview(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.PrintPreview", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Checks for and returns the cookie value with the specified name.
	/// </summary>
	/// <param name="name">The name of the cookie value to return.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<string> ReadCookie(this IJSRuntime jSRuntime, string name, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<string>("easyBlazorBulma.ReadCookie", token ?? CancellationToken.None, name);
	}

	/// <summary>
	/// Reads and returns text from the system clipboard.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<string> ReadClipboard(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<string>("easyBlazorBulma.ReadClipboard", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Checks for and returns the history state for the current URI.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<string?> ReadHistoryState(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		var data = await jSRuntime.InvokeAsync<object?>("easyBlazorBulma.ReadHistoryState", token ?? CancellationToken.None);
		return data?.ToString();
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
	/// Refreshes the page.
	/// </summary>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task Reload(this IJSRuntime jSRuntime, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.Reload", token ?? CancellationToken.None);
	}

	/// <summary>
	/// Saves the provided stream to the database.
	/// </summary>
	/// <param name="name">The database to read from.</param>
	/// <param name="store">The table in the database to read from.</param>
	/// <param name="version">The version number of the database.</param>
	/// <param name="id">The key the image is stored under.</param>
	/// <param name="reference">A reference to the stream containing the data.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> SaveBlobToIndexedDb(this IJSRuntime jSRuntime, string name, string store, int version, string id, DotNetStreamReference reference, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.SaveBlobToIndexedDb", token ?? CancellationToken.None, name, store, version, id, reference);
	}

	/// <summary>
	/// Selects the inner text of the specified element.
	/// </summary>
	/// <param name="id">The identity tag of the element to select.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> SelectElement(this IJSRuntime jSRuntime, string id, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.SelectElement", token ?? CancellationToken.None, id);
	}

	/// <summary>
	/// Scrolls the browser window to the provided coordinates.
	/// </summary>
	/// <param name="x">The horizontal coordinate to scroll to.</param>
	/// <param name="y">The vertical coordinate to scroll to.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task SetScroll(this IJSRuntime jSRuntime, int x, int y, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.SetScroll", token ?? CancellationToken.None, x, y);
	}

	/// <summary>
	/// Starts a camera stream on the specified video element.
	/// </summary>
	/// <param name="id">The id value of the element to link the camera stream to.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> StartCamera(this IJSRuntime jSRuntime, string id, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.StartCamera", token ?? CancellationToken.None, id);
	}

	/// <summary>
	/// Stops an active camera stream on the specified video element.
	/// </summary>
	/// <param name="id">The id value of the element to stop the camera stream on.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> StopCamera(this IJSRuntime jSRuntime, string id, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.StopCamera", token ?? CancellationToken.None, id);
	}

	/// <summary>
	/// Takes a photo on the active camera stream and returns the result as a byte array.
	/// </summary>
	/// <param name="videoId">The id value of the element the stream is running on.</param>
	/// <param name="canvasId">The id value of a canvas element to process the image.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<byte[]> TakePhoto(this IJSRuntime jSRuntime, string videoId, string canvasId, CancellationToken? token = null)
	{
		var data = await jSRuntime.InvokeAsync<string?>("easyBlazorBulma.TakePhoto", token ?? CancellationToken.None, videoId, canvasId, null, "image/jpeg");

		if (string.IsNullOrWhiteSpace(data))
			return [];

		return Convert.FromBase64String(data.Split(',').Last());
	}

	/// <summary>
	/// Takes a photo on the active camera stream and returns the result as a byte array.
	/// </summary>
	/// <param name="videoId">The id value of the element the stream is running on.</param>
	/// <param name="canvasId">The id value of a canvas element to process the image.</param>
	/// <param name="imageId">The id value of an image element to preview the image.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<byte[]> TakePhoto(this IJSRuntime jSRuntime, string videoId, string canvasId, string? imageId, CancellationToken? token = null)
	{
		var data = await jSRuntime.InvokeAsync<string?>("easyBlazorBulma.TakePhoto", token ?? CancellationToken.None, videoId, canvasId, imageId, "image/jpeg");

		if (string.IsNullOrWhiteSpace(data))
			return [];

		return Convert.FromBase64String(data.Split(',').Last());
	}

	/// <summary>
	/// Takes a photo on the active camera stream and returns the result as a byte array.
	/// </summary>
	/// <param name="videoId">The id value of the element the stream is running on.</param>
	/// <param name="canvasId">The id value of a canvas element to process the image.</param>
	/// <param name="imageId">The id value of an image element to preview the image.</param>
	/// <param name="mimeType">The mime type to save the image data as.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<byte[]> TakePhoto(this IJSRuntime jSRuntime, string videoId, string canvasId, string? imageId, string mimeType, CancellationToken? token = null)
	{
		var data = await jSRuntime.InvokeAsync<string?>("easyBlazorBulma.TakePhoto", token ?? CancellationToken.None, videoId, canvasId, imageId, mimeType);

		if (string.IsNullOrWhiteSpace(data))
			return [];

		return Convert.FromBase64String(data.Split(',').Last());
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
	/// Revokes the blob content loaded into the specified element.
	/// </summary>
	/// <param name="id">The id of the element to unload.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task UnloadBlob(this IJSRuntime jSRuntime, string id, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.UnloadBlob", token ?? CancellationToken.None, id);
	}

	/// <summary>
	/// Updates the current URI to the provided value without adding a state entry.
	/// </summary>
	/// <param name="uri">The value to update the URI to.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task UpdateCurrentUri(this IJSRuntime jSRuntime, string uri, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.UpdateCurrentUri", token ?? CancellationToken.None, uri);
	}

	/// <summary>
	/// Creates or replaces a cookie with the provided name.
	/// </summary>
	/// <param name="name">The text identifier for the cookie.</param>
	/// <param name="value">The value to store in the cookie.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> WriteCookie(this IJSRuntime jSRuntime, string name, string? value, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.WriteCookie", token ?? CancellationToken.None, name, value);
	}

	/// <summary>
	/// Creates or replaces a cookie with the provided name.
	/// </summary>
	/// <param name="name">The text identifier for the cookie.</param>
	/// <param name="value">The value to store in the cookie.</param>
	/// <param name="validFor">The duration in milliseconds the cookie is valid for.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> WriteCookie(this IJSRuntime jSRuntime, string name, string? value, int validFor, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.WriteCookie", token ?? CancellationToken.None, name, value, validFor);
	}

	/// <summary>
	/// Creates or replaces a cookie with the provided name.
	/// </summary>
	/// <param name="name">The text identifier for the cookie.</param>
	/// <param name="value">The value to store in the cookie.</param>
	/// <param name="path">A web path root to restrict the cookie to.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> WriteCookie(this IJSRuntime jSRuntime, string name, string? value, string path, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.WriteCookie", token ?? CancellationToken.None, name, value, null, path);
	}

	/// <summary>
	/// Creates or replaces a cookie with the provided name.
	/// </summary>
	/// <param name="name">The text identifier for the cookie.</param>
	/// <param name="value">The value to store in the cookie.</param>
	/// <param name="validFor">The duration in milliseconds the cookie is valid for.</param>
	/// <param name="path">A web path root to restrict the cookie to.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> WriteCookie(this IJSRuntime jSRuntime, string name, string? value, int validFor, string path, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.WriteCookie", token ?? CancellationToken.None, name, value, validFor, path);
	}

	/// <summary>
	/// Writes text to the system clipboard.
	/// </summary>
	/// <param name="text">The text to write to the clipboard.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task<bool> WriteClipboard(this IJSRuntime jSRuntime, string text, CancellationToken? token = null)
	{
		return await jSRuntime.InvokeAsync<bool>("easyBlazorBulma.WriteClipboard", token ?? CancellationToken.None, text);
	}

	/// <summary>
	/// Creates or updates a history state with the provided value.
	/// </summary>
	/// <param name="value">The serialized data to store in the history.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task WriteHistoryState(this IJSRuntime jSRuntime, string value, CancellationToken? token = null)
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.WriteHistoryState", token ?? CancellationToken.None, value);
	}

	/// <summary>
	/// Creates or updates a history state with the provided value.
	/// </summary>
	/// <param name="value">The data to store in the history.</param>
	/// <param name="options">Options to pass to the JSON serializer.</param>
	/// <param name="token">A cancellation token to abort the request.</param>
	public async static Task WriteHistoryState<TState>(this IJSRuntime jSRuntime, TState value, JsonSerializerOptions? options = null, CancellationToken? token = null) where TState : class
	{
		await jSRuntime.InvokeVoidAsync("easyBlazorBulma.WriteHistoryState", token ?? CancellationToken.None, JsonSerializer.Serialize(value, options));
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
