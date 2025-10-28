using easy_core;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma;

/// <summary>
/// A calendar to display date grid based content.
/// </summary>
/// <remarks>
/// There are 2 additional attributes that can be used: column-class and table-class. They will apply CSS classes to the resulting elements as per their names.
/// </remarks>
public partial class Calendar : ComponentBase
{
	/// <summary>
	/// A collection of dates within the months to be displayed.
	/// </summary>
	/// <remarks>
	/// Each month should have only one day within it and can be any day in the month to display.
	/// </remarks>
	[Parameter]
	public required List<DateOnly> Months { get; set; }

	/// <summary>
	/// The first day of the week to display.
	/// </summary>
	[Parameter]
	public DayOfWeek StartOfWeek { get; set; } = DayOfWeek.Sunday;

	/// <summary>
	/// The default height of rows within the calendars in pixels.
	/// </summary>
	[Parameter]
	[Range(10, 10_000)]
	public int RowHeight { get; set; } = 100;

	/// <summary>
	/// The date format string to apply to the month titles.
	/// </summary>
	/// <remarks>
	/// Set to null to hide the month titles.
	/// </remarks>
	[Parameter]
	public string? MonthFormat { get; set; } = "Y";

	/// <summary>
	/// The date format string to apply to day of the week titles.
	/// </summary>
	[Parameter]
	public string DayFormat { get; set; } = "dddd";

	/// <summary>
	/// Specifies whether to invoke for all days or only those within the current month.
	/// </summary>
	/// <remarks>
	/// Example: 2024-10-01 will be displayed on the 2024-09 calendar; when this value is true it will behave the same as other dates, when this value is false it will be an empty cell.
	/// </remarks>
	[Parameter]
	public bool ShowAllDays { get; set; }

	/// <summary>
	/// Specifies whether to print each calendar month on its own page.
	/// </summary>
	[Parameter]
	public bool PrintSinglePage { get; set; } = true;

	/// <summary>
	/// Content to display for each day in the months.
	/// </summary>
	[Parameter]
	public required RenderFragment<DateOnly> Days { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private string MainCssClass => Months.Count > 1 ? string.Join(' ', "columns is-multiline is-variable is-1 pt-2", AdditionalAttributes.GetClass("class")) : string.Join(' ', "pt-2", AdditionalAttributes.GetClass("class"));

	private string ColumnCssClass
	{
		get
		{
			var css = "";

			if (Months.Count > 1)
				css += " column is-12-desktop is-12-widescreen is-6-fullhd is-4-4k";

			if (PrintSinglePage)
				css += " is-break-after is-fullwidth-print";
			else
				css += " is-break-avoid";

			return string.Join(' ', css, AdditionalAttributes.GetClass("column-class"));
		}
	}

	private string TableCssClass => string.Join(' ', "is-size-7 is-fullwidth is-bordered", AdditionalAttributes.GetClass("table-class"));

	private IEnumerable<DateOnly> GetWeeksInMonth(DateOnly month)
	{
		var current = new DateOnly(month.Year, month.Month, 1).GetPreviousWeekday(StartOfWeek);
		var last = new DateOnly(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month)).GetPreviousWeekday(StartOfWeek).AddDays(6);

		do
		{
			yield return current;
			current = current.AddDays(7);
		} while (current < last);
	}
}
