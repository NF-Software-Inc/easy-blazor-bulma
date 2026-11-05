using System.Reflection;
using easy_blazor_bulma;
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
			string? cssClass = null)
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
		}

		public string CssClass => (string)Get("MainCssClass")!;

		public Dictionary<string, object?> Options => Invoke<Dictionary<string, object?>>("BuildOptions");

		public string Fingerprint => Invoke<string>("BuildOptionsFingerprint");

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
	}
}
