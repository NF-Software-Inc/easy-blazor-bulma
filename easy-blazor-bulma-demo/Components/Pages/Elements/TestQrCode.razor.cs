using easy_blazor_bulma;
using easy_core;
using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma_demo.Components.Pages.Elements;

public partial class TestQrCode : ComponentBase
{
	private string Text = "https://github.com/NF-Software-Inc/easy-blazor-bulma";

	private int Width = 256;

	private int Height = 256;

	private string ColorDark = "#000000";

	private string ColorLight = "#ffffff";

	private QrCodeCorrectLevel CorrectLevel = QrCodeCorrectLevel.H;

	private bool UseSvg;

	private readonly List<QrCodeCorrectLevel> Levels = Enum.GetValues<QrCodeCorrectLevel>().ToList();

	private string FormatLevel(QrCodeCorrectLevel level) => level.GetValueDisplayName();
}
