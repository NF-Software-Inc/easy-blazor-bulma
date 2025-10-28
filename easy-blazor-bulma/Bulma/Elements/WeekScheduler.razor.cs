using easy_core;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma;

/// <summary>
/// A single-week calendar to display per-day and hour based content.
/// </summary>
/// <remarks>
/// There is 1 additional attribute that can be used: table-class. It will apply CSS classes to the resulting element as per its name.
/// </remarks>
public partial class WeekScheduler : ComponentBase
{
	/// <summary>
	/// A date within the week to be displayed.
	/// </summary>
	/// <remarks>
	/// Can be any day within the week to display.
	/// </remarks>
	[Parameter]
	public required DateOnly DayInWeek { get; set; }

	/// <summary>
	/// The first day of the week to display.
	/// </summary>
	[Parameter]
	public DayOfWeek StartOfWeek { get; set; } = DayOfWeek.Sunday;

	/// <summary>
	/// The first hour to display for each day.
	/// </summary>
	[Parameter]
	[Range(0, 23)]
	public int StartHour { get; set; } = 0;

	/// <summary>
	/// The last hour to display for each day.
	/// </summary>
	[Parameter]
	[Range(0, 23)]
	[GreaterThanOrEqual(nameof(StartHour))]
	public int EndHour { get; set; } = 23;

	/// <summary>
	/// The default height of rows within the days in pixels.
	/// </summary>
	[Parameter]
	[Range(10, 10_000)]
	public int RowHeight { get; set; } = 60;

	/// <summary>
	/// The date format string to apply to the week range title.
	/// </summary>
	/// <remarks>
	/// Set to null to hide the week range title.
	/// </remarks>
	[Parameter]
	public string? WeekFormat { get; set; } = "d";

	/// <summary>
	/// The date format string to apply to day of the week titles.
	/// </summary>
	[Parameter]
	public string DayFormat { get; set; } = "dddd";

	/// <summary>
	/// The time format string to apply to hour of the day labels.
	/// </summary>
	/// <remarks>
	/// Set to null to hide the hour of day column.
	/// </remarks>
	[Parameter]
	public string? HourFormat { get; set; } = "t";

	/// <summary>
	/// Content to display for each hour in the days of the week.
	/// </summary>
	[Parameter]
	public required RenderFragment<DateTime> Hours { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private string TableCssClass => string.Join(' ', "is-size-7 is-fullwidth is-bordered", AdditionalAttributes.GetClass("table-class"));
}
