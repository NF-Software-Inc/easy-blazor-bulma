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
	/// Whether to draw the QR code on the first render.
	/// </summary>
	/// <remarks>
	/// Must call <see cref="Initialize"/> manually if false.
	/// </remarks>
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

	private bool IsInitialized;
	private bool PendingOptionsUpdate;
	private string? LastOptionsFingerprint;

	private readonly SemaphoreSlim LayoutGate = new(1, 1);

	private string MainCssClass => string.Join(' ', "qr-code", AdditionalAttributes.GetValue("class"));

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && InitializeOnRender)
			_ = await Initialize();

		if (PendingOptionsUpdate && IsInitialized)
		{
			PendingOptionsUpdate = false;

			_ = await Update();
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
	/// Prepares the component for use and creates or replaces the QR code instance.
	/// </summary>
	/// <remarks>
	/// Only required when <see cref="InitializeOnRender"/> is false; subsequent calls are ignored once initialized.
	/// </remarks>
	public async Task<bool> Initialize()
	{
		if (IsInitialized)
			return true;

		var ready = await JsRuntime.LoadScript("_content/Easy.Blazor.Bulma/js/qrcode.js", "QRCode", TimeSpan.FromSeconds(1));

		if (ready == false)
			return false;

		IsInitialized = true;

		if (IsInitialized)
			LastOptionsFingerprint = BuildOptionsFingerprint();

		return await JsRuntime.QrCodeInitialize(ContainerId, BuildOptions());
	}

	/// <summary>
	/// Regenerates the QR code from the current value and options.
	/// </summary>
	public async Task<bool> Refresh()
	{
		if (IsInitialized == false)
			return false;

		return await Update();
	}

	/// <summary>
	/// Clears the current drawing without changing <see cref="Value"/>.
	/// </summary>
	/// <remarks>
	/// The next value or option change redraws from the current parameters.
	/// </remarks>
	public async Task<bool> Clear()
	{
		if (IsInitialized == false)
			return false;

		await LayoutGate.WaitAsync();

		try
		{
			return await JsRuntime.QrCodeClear(ContainerId);
		}
		finally
		{
			LayoutGate.Release();
		}
	}

	/// <summary>
	/// Applies a value-only change or recreates the instance when drawing options changed.
	/// </summary>
	private async Task<bool> Update()
	{
		if (IsInitialized == false)
			return false;

		await LayoutGate.WaitAsync();

		try
		{
			bool updated;

			if (FingerprintOptionsChanged(LastOptionsFingerprint, BuildOptionsFingerprint()))
				updated = await JsRuntime.QrCodeInitialize(ContainerId, BuildOptions());
			else
				updated = await JsRuntime.QrCodeMakeCode(ContainerId, Value);

			if (updated)
				LastOptionsFingerprint = BuildOptionsFingerprint();

			return updated;
		}
		finally
		{
			LayoutGate.Release();
		}
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

	private static bool FingerprintOptionsChanged(string? fingerprint1, string fingerprint2)
	{
		var value1 = fingerprint1 != null ? string.Join('|', fingerprint1.Split('|').Where(x => x.StartsWith("text=") == false)) : null;
		var value2 = string.Join('|', fingerprint2.Split('|').Where(x => x.StartsWith("text=") == false));

		return string.Equals(value1, value2, StringComparison.Ordinal) == false;
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (IsInitialized)
			await JsRuntime.QrCodeDestroy(ContainerId);

		LayoutGate.Dispose();
		GC.SuppressFinalize(this);
	}
}
