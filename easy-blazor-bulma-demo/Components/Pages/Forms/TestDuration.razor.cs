using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma_demo.Components.Pages.Forms;

public partial class TestDuration : ComponentBase
{
	private readonly PageModel InputModel = new();

	private readonly InputDurationOptions Options = InputDurationOptions.HoverPopout | InputDurationOptions.ShowResetButton | InputDurationOptions.ShowHours | InputDurationOptions.ShowMinutes | InputDurationOptions.ShowSeconds | InputDurationOptions.ShowMilliseconds;
	private readonly InputDurationOptions SignedOptions = InputDurationOptions.ClickPopout | InputDurationOptions.PopoutBottom | InputDurationOptions.PopoutLeft | InputDurationOptions.UpdateOnPopoutChange | InputDurationOptions.UseAutomaticStatusColors | InputDurationOptions.ValidateTextInput | InputDurationOptions.ShowResetButton | InputDurationOptions.ShowDays | InputDurationOptions.ShowHours | InputDurationOptions.ShowMinutes | InputDurationOptions.AllowNegative | InputDurationOptions.AllowGreaterThan24Hours;
	private readonly InputDurationOptions RollupOptions = InputDurationOptions.ClickPopout | InputDurationOptions.PopoutBottom | InputDurationOptions.PopoutLeft | InputDurationOptions.UpdateOnPopoutChange | InputDurationOptions.UseAutomaticStatusColors | InputDurationOptions.ValidateTextInput | InputDurationOptions.ShowResetButton | InputDurationOptions.ShowMinutes | InputDurationOptions.ShowSeconds | InputDurationOptions.AllowGreaterThan24Hours | InputDurationOptions.DisplayHoursAsMinutes;
	private readonly TooltipOptions TooltipDisplayMode = TooltipOptions.Right | TooltipOptions.HasArrow | TooltipOptions.Multiline;

	/// <summary>
	/// Formats a duration as a readable summary to demonstrate the Formatter parameter.
	/// </summary>
	/// <param name="value">The duration to format.</param>
	private string? FormatDuration(TimeSpan? value)
	{
		if (value == null)
			return null;

		var sign = value.Value < TimeSpan.Zero ? "-" : string.Empty;
		var duration = value.Value.Duration();

		return $"{sign}{duration.Days}d {duration.Hours}h {duration.Minutes}m";
	}

	private class PageModel
	{
		[Display(Name = "Time Span Test", Description = "Input to test a non-nullable TimeSpan value.")]
		public TimeSpan TimeSpanTest { get; set; } = TimeSpan.Zero;

		[Display(Name = "Time Only Test", Description = "Input to test a nullable TimeOnly value.")]
		public TimeOnly? TimeOnlyTest { get; set; }

		[Display(Name = "Signed Test", Description = "Input to test negative durations and durations beyond 24 hours. Days is the largest displayed column, so it carries the total and the sign.")]
		public TimeSpan? SignedTest { get; set; } = TimeSpan.FromHours(-30);

		[Display(Name = "Rollup Test", Description = "Input to test roll up into the largest displayed column. Minutes is the largest column shown, so it displays the total number of minutes rather than the minutes component.")]
		public TimeSpan RollupTest { get; set; } = TimeSpan.FromMinutes(135);

		[Display(Name = "Formatted Test", Description = "Input to test the Formatter function. The value is displayed as a readable summary while the input does not have focus.")]
		public TimeSpan? FormattedTest { get; set; } = TimeSpan.FromHours(50);
	}
}
