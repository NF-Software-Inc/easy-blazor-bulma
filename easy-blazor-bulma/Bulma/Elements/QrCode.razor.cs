using easy_core;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace easy_blazor_bulma;

/// <summary>
/// Displays a QR code image generated from text.
/// </summary>
public partial class QrCode : ComponentBase, IAsyncDisposable
{
	/// <summary>
	/// Text encoded into the QR code.
	/// </summary>
	[Parameter]
	public string? Value { get; set; }

	/// <summary>
	/// Width of the generated code in pixels.
	/// </summary>
	[Parameter]
	public int Width { get; set; } = 256;

	/// <summary>
	/// Height of the generated code in pixels.
	/// </summary>
	[Parameter]
	public int Height { get; set; } = 256;

	/// <summary>
	/// CSS color used for dark modules.
	/// </summary>
	[Parameter]
	public string? ColorDark { get; set; } = "#000000";

	/// <summary>
	/// CSS color used for light modules.
	/// </summary>
	[Parameter]
	public string? ColorLight { get; set; } = "#ffffff";

	/// <summary>
	/// Error correction level.
	/// </summary>
	[Parameter]
	public QrCodeCorrectLevel CorrectLevel { get; set; } = QrCodeCorrectLevel.H;

	/// <summary>
	/// Renders an SVG instead of a canvas when true.
	/// </summary>
	[Parameter]
	public bool UseSvg { get; set; }

	/// <summary>
	/// Whether to draw the QR code on the first render. Set to false to delay drawing until <see cref="Refresh"/> is called.
	/// </summary>
	[Parameter]
	public bool InitializeOnRender { get; set; } = true;

	/// <summary>
	/// Additional attributes applied to the root container.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	[Inject]
	private IJSRuntime JsRuntime { get; init; } = default!;

	private readonly string[] Filter = ["class"];
	private readonly string ContainerId = $"qr-code-{Guid.NewGuid().ToHtmlId():N}";
	private readonly SemaphoreSlim Gate = new(1, 1);

	private bool IsInitialized;
	private bool PendingOptionsUpdate;
	private int IsDisposed;
	private string? LastOptionsFingerprint;
	private string? LastRenderOptionsFingerprint;

	private string MainCssClass => string.Join(' ', "qr-code", AdditionalAttributes.GetValue("class"));

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && InitializeOnRender)
			await Initialize();

		if (PendingOptionsUpdate && IsInitialized)
		{
			PendingOptionsUpdate = false;
			await Update();
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
	/// Regenerates the QR code from the current value and options.
	/// </summary>
	/// <returns>True when the code is drawn or cleared; otherwise, false.</returns>
	public async Task<bool> Refresh()
	{
		if (IsInitialized == false)
			return false;

		return await Update();
	}

	/// <summary>
	/// Clears the current drawing without changing <see cref="Value"/>.
	/// </summary>
	/// <returns>True when the drawing is cleared; otherwise, false.</returns>
	/// <remarks>
	/// The next value or option change redraws from the current parameters.
	/// </remarks>
	public async Task<bool> Clear()
	{
		if (IsInitialized == false || await TryEnterGate() == false)
			return false;

		try
		{
			if (IsInitialized == false)
				return false;

			return await JsRuntime.QrCodeClear(ContainerId);
		}
		finally
		{
			Gate.Release();
		}
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (Interlocked.Exchange(ref IsDisposed, 1) != 0)
			return;

		try
		{
			await Gate.WaitAsync();

			try
			{
				if (IsInitialized)
				{
					IsInitialized = false;
					await JsRuntime.QrCodeDestroy(ContainerId);
				}
			}
			finally
			{
				Gate.Release();
			}
		}
		catch (JSDisconnectedException)
		{
		}
		catch (ObjectDisposedException)
		{
		}
		finally
		{
			Gate.Dispose();
			GC.SuppressFinalize(this);
		}
	}

	/// <summary>
	/// Prepares the component for use and creates or replaces the QR code instance.
	/// </summary>
	private async Task<bool> Initialize()
	{
		if (await TryEnterGate() == false)
			return false;

		try
		{
			if (Volatile.Read(ref IsDisposed) != 0)
				return false;

			var ready = await JsRuntime.LoadScript("_content/Easy.Blazor.Bulma/js/qrcode.js", "QRCode", TimeSpan.FromSeconds(1));

			if (ready == false || Volatile.Read(ref IsDisposed) != 0)
				return false;

			IsInitialized = await JsRuntime.QrCodeInitialize(ContainerId, BuildOptions());

			if (IsInitialized)
			{
				RememberOptions();

				if (string.Equals(LastOptionsFingerprint, BuildOptionsFingerprint(), StringComparison.Ordinal) == false)
					PendingOptionsUpdate = true;
			}

			return IsInitialized;
		}
		finally
		{
			Gate.Release();
		}
	}

	/// <summary>
	/// Applies a value-only change or recreates the instance when drawing options changed.
	/// </summary>
	private async Task<bool> Update()
	{
		if (Volatile.Read(ref IsDisposed) != 0)
			return false;

		if (string.Equals(LastRenderOptionsFingerprint, BuildRenderOptionsFingerprint(), StringComparison.Ordinal) == false)
			return await Initialize();

		if (await TryEnterGate() == false)
			return false;

		try
		{
			if (IsInitialized == false || Volatile.Read(ref IsDisposed) != 0)
				return false;

			var updated = await JsRuntime.QrCodeMakeCode(ContainerId, Value);

			if (updated)
				RememberOptions();

			return updated;
		}
		finally
		{
			Gate.Release();
		}
	}

	/// <summary>
	/// Enters the interop gate unless the component has already been disposed.
	/// </summary>
	/// <returns>True when the caller owns the gate and must release it.</returns>
	private async Task<bool> TryEnterGate()
	{
		if (Volatile.Read(ref IsDisposed) != 0)
			return false;

		try
		{
			await Gate.WaitAsync();
		}
		catch (ObjectDisposedException)
		{
			return false;
		}

		if (Volatile.Read(ref IsDisposed) == 0)
			return true;

		Gate.Release();
		return false;
	}

	/// <summary>
	/// Builds the options for the component.
	/// </summary>
	private Dictionary<string, object?> BuildOptions()
	{
		var options = new Dictionary<string, object?>
		{
			["width"] = Width,
			["height"] = Height,
			["colorDark"] = string.IsNullOrWhiteSpace(ColorDark) ? "#000000" : ColorDark,
			["colorLight"] = string.IsNullOrWhiteSpace(ColorLight) ? "#ffffff" : ColorLight,
			["correctLevel"] = (int)CorrectLevel,
			["useSVG"] = UseSvg
		};

		if (string.IsNullOrEmpty(Value) == false)
			options["text"] = Value;

		return options;
	}

	/// <summary>
	/// Builds a fingerprint of every option, including the encoded text.
	/// </summary>
	private string BuildOptionsFingerprint()
	{
		return string.Join('|', BuildOptions().Select(x => $"{x.Key}={x.Value}"));
	}

	/// <summary>
	/// Builds a fingerprint of drawing options that require a new instance.
	/// </summary>
	private string BuildRenderOptionsFingerprint()
	{
		return string.Join('|', BuildOptions().Where(x => x.Key != "text").Select(x => $"{x.Key}={x.Value}"));
	}

	/// <summary>
	/// Stores fingerprints for the options last applied.
	/// </summary>
	private void RememberOptions()
	{
		LastOptionsFingerprint = BuildOptionsFingerprint();
		LastRenderOptionsFingerprint = BuildRenderOptionsFingerprint();
	}
}
