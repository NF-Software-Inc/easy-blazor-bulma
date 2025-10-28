using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// A clock to display the current time.
/// </summary>
public sealed partial class Clock : ComponentBase, IDisposable
{
	/// <summary>
	/// The standard <see cref="DateTime"/> format to apply.
	/// </summary>
	[Parameter]
	public string Format { get; set; } = "g";

	/// <summary>
	/// Specifies whether the component will be contained in the navbar.
	/// </summary>
	[Parameter]
	public bool IsNavbarItem { get; set; } = true;

	/// <summary>
	/// Specifies whether to show the week of the year before the date and time.
	/// </summary>
	[Parameter]
	public bool ShowWeek { get; set; }

	/// <summary>
	/// The amount of time between clock refreshes.
	/// </summary>
	[Parameter]
	public TimeSpan TickDuration { get; set; } = TimeSpan.FromMilliseconds(1_000);

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private Timer ClockTimer = default!;

	private string FullCssClass
	{
		get
		{
			var css = "";

			if (IsNavbarItem)
				css += " navbar-item";

			return string.Join(' ', css.TrimStart(), CssClass);
		}
	}

	private string? CssClass
	{
		get
		{
			if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("class", out var css) && string.IsNullOrWhiteSpace(Convert.ToString(css, CultureInfo.InvariantCulture)) == false)
				return css.ToString();

			return null;
		}
	}

	/// <inheritdoc/>
	protected override void OnInitialized()
	{
		ClockTimer = new Timer(Tick, null, TimeSpan.Zero, TickDuration);
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		if (ClockTimer != null)
		{
			ClockTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
			ClockTimer.Dispose();
		}
	}

	private async void Tick(object? _)
	{
		await InvokeAsync(StateHasChanged);
	}
}
