using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using easy_blazor_bulma;
using Microsoft.JSInterop;
using Xunit;

namespace easy_blazor_bulma_test;

public class QrCodeTests
{
	[Fact]
	public void CorrectLevelValuesMatchQrCodeJs()
	{
		Assert.Equal(0, (int)QrCodeCorrectLevel.M);
		Assert.Equal(1, (int)QrCodeCorrectLevel.L);
		Assert.Equal(2, (int)QrCodeCorrectLevel.H);
		Assert.Equal(3, (int)QrCodeCorrectLevel.Q);
	}

	[Fact]
	public void DefaultOptionsMatchQrCodeJsConstructor()
	{
		var options = new TestQrCode().Options;

		Assert.Equal(256, options["width"]);
		Assert.Equal(256, options["height"]);
		Assert.Equal("#000000", options["colorDark"]);
		Assert.Equal("#ffffff", options["colorLight"]);
		Assert.Equal((int)QrCodeCorrectLevel.H, options["correctLevel"]);
		Assert.Equal(false, options["useSVG"]);
		Assert.False(options.ContainsKey("text"));
	}

	[Fact]
	public void ValueIsPassedAsTextOption()
	{
		var options = new TestQrCode("https://example.test/path").Options;

		Assert.Equal("https://example.test/path", options["text"]);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public void EmptyValueOmitsTextSoTheLibraryDoesNotDraw(string? value)
	{
		var options = new TestQrCode(value).Options;

		Assert.False(options.ContainsKey("text"));
	}

	[Fact]
	public void DrawingOptionsUseLibraryPropertyNames()
	{
		var options = new TestQrCode(
			value: "payload",
			width: 128,
			height: 96,
			colorDark: "#112233",
			colorLight: "white",
			correctLevel: QrCodeCorrectLevel.Q,
			useSvg: true).Options;

		Assert.Equal(128, options["width"]);
		Assert.Equal(96, options["height"]);
		Assert.Equal("#112233", options["colorDark"]);
		Assert.Equal("white", options["colorLight"]);
		Assert.Equal((int)QrCodeCorrectLevel.Q, options["correctLevel"]);
		Assert.Equal(true, options["useSVG"]);
		Assert.Equal("payload", options["text"]);
	}

	[Theory]
	[InlineData(null, "#000000")]
	[InlineData("", "#000000")]
	[InlineData("   ", "#000000")]
	public void MissingDarkColorFallsBackToLibraryDefault(string? color, string expected)
	{
		var options = new TestQrCode(colorDark: color).Options;

		Assert.Equal(expected, options["colorDark"]);
	}

	[Fact]
	public void CssClassIncludesQrCodeAndAdditionalClass()
	{
		var css = new TestQrCode(cssClass: "mx-auto").CssClass;
		var tokens = css.Split(' ', StringSplitOptions.RemoveEmptyEntries);

		Assert.Contains("qr-code", tokens);
		Assert.Contains("mx-auto", tokens);
	}

	[Fact]
	public void DrawingOptionChangeRequiresANewInstance()
	{
		var original = new TestQrCode("one", width: 128);
		var updated = new TestQrCode("one", width: 256, useSvg: true);

		Assert.NotEqual(original.Fingerprint, updated.Fingerprint);
	}

	[Theory]
	[InlineData(null, false, "qrcode.png")]
	[InlineData("", false, "qrcode.png")]
	[InlineData("   ", false, "qrcode.png")]
	[InlineData(null, true, "qrcode.svg")]
	[InlineData("my-code", false, "my-code.png")]
	[InlineData("my-code", true, "my-code.svg")]
	[InlineData("my-code.png", false, "my-code.png")]
	[InlineData("my-code.png", true, "my-code.svg")]
	[InlineData("my-code.SVG", false, "my-code.png")]
	[InlineData(".png", false, "qrcode.png")]
	[InlineData(".svg", true, "qrcode.svg")]
	[InlineData("release-v1.2", false, "release-v1.2.png")]
	[InlineData("  custom.svg  ", true, "custom.svg")]
	public void ResolveFileNameNormalizesExtensionToActiveFormat(string? fileName, bool useSvg, string expected)
	{
		var component = new TestQrCode("https://example.test", useSvg: useSvg);

		Assert.Equal(expected, component.ResolveDownloadFileName(fileName));
	}

	[Fact]
	public async Task DownloadReturnsFalseWhenNotInitialized()
	{
		var js = new RecordingJsRuntime();
		var component = new TestQrCode("https://example.test", jsRuntime: js);

		var result = await component.Download();

		Assert.False(result);
		Assert.Empty(js.Invocations);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public async Task DownloadReturnsFalseWhenValueIsEmpty(string? value)
	{
		var js = new RecordingJsRuntime();
		var component = new TestQrCode(value, jsRuntime: js);
		await component.Initialize();
		js.Invocations.Clear();

		var result = await component.Download();

		Assert.False(result);
		Assert.Empty(js.Invocations);
	}

	[Fact]
	public async Task DownloadInvokesJsWithPngExtensionWhenUseSvgIsFalse()
	{
		var js = new RecordingJsRuntime();
		var component = new TestQrCode("https://example.test", useSvg: false, jsRuntime: js);
		await component.Initialize();
		js.Invocations.Clear();

		var result = await component.Download();

		Assert.True(result);
		var call = Assert.Single(js.Invocations);
		Assert.Equal("easyBlazorBulma.QrCode.Download", call.Identifier);
		Assert.Equal(component.ElementId, call.Args?[0]);
		Assert.Equal("qrcode.png", call.Args?[1]);
	}

	[Fact]
	public async Task DownloadInvokesJsWithSvgExtensionWhenUseSvgIsTrue()
	{
		var js = new RecordingJsRuntime();
		var component = new TestQrCode("https://example.test", useSvg: true, jsRuntime: js);
		await component.Initialize();
		js.Invocations.Clear();

		var result = await component.Download("badge.png");

		Assert.True(result);
		var call = Assert.Single(js.Invocations);
		Assert.Equal("easyBlazorBulma.QrCode.Download", call.Identifier);
		Assert.Equal(component.ElementId, call.Args?[0]);
		Assert.Equal("badge.svg", call.Args?[1]);
	}

	private sealed class RecordingJsRuntime : IJSRuntime
	{
		public List<(string Identifier, object?[]? Args)> Invocations { get; } = [];

		public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object?[]? args)
		{
			return InvokeAsync<TValue>(identifier, CancellationToken.None, args);
		}

		public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
		{
			Invocations.Add((identifier, args));

			object? result = typeof(TValue) == typeof(bool) ? true : default(TValue);
			return new ValueTask<TValue>((TValue)result!);
		}
	}

	private sealed class TestQrCode : QrCode
	{
		public TestQrCode(
			string? value = null,
			int width = 256,
			int height = 256,
			string? colorDark = "#000000",
			string? colorLight = "#ffffff",
			QrCodeCorrectLevel correctLevel = QrCodeCorrectLevel.H,
			bool useSvg = false,
			string? cssClass = null,
			IJSRuntime? jsRuntime = null)
		{
			Value = value;
			Width = width;
			Height = height;
			ColorDark = colorDark;
			ColorLight = colorLight;
			CorrectLevel = correctLevel;
			UseSvg = useSvg;

			if (cssClass != null)
				AdditionalAttributes = new Dictionary<string, object> { { "class", cssClass } };

			if (jsRuntime != null)
			{
				var jsProperty = typeof(QrCode).GetProperty("JsRuntime", BindingFlags.Instance | BindingFlags.NonPublic);
				Assert.NotNull(jsProperty);
				jsProperty.SetValue(this, jsRuntime);
			}
		}

		public string ElementId => (string)GetField("ContainerId")!;

		public string CssClass => (string)Get("MainCssClass")!;

		public Dictionary<string, object?> Options => Invoke<Dictionary<string, object?>>("BuildOptions");

		public string Fingerprint => Invoke<string>("BuildOptionsFingerprint");

		public string ResolveDownloadFileName(string? fileName)
		{
			var method = typeof(QrCode).GetMethod("ResolveFileName", BindingFlags.Instance | BindingFlags.NonPublic);

			return (string)method!.Invoke(this, [fileName])!;
		}

		private T Invoke<T>(string name)
		{
			var method = typeof(QrCode).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);

			return (T)method!.Invoke(this, null)!;
		}

		private object? Get(string name)
		{
			var property = typeof(QrCode).GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);

			return property!.GetValue(this);
		}

		private object? GetField(string name)
		{
			var field = typeof(QrCode).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);

			return field!.GetValue(this);
		}
	}
}
