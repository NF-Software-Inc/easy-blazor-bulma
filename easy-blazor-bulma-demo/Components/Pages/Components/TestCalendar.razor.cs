using easy_core;
using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma_demo.Components.Pages.Components;

public partial class TestCalendar : ComponentBase
{
	private DateTime SelectedDate { get; set; } = DateTime.Today;
	private DayOfWeek StartOfWeek { get; init; } = DayOfWeek.Monday;

	private readonly List<DateOnly> Months = [DateTime.Today.ToDateOnly()];
	private readonly List<Appointment> Appointments = [];

	/// <summary>
	/// Represents a preset for the column-class attribute of the Calendar component, including its value, label, and tooltip.
	/// </summary>
	/// <param name="Value">The value of the column-class preset.</param>
	/// <param name="Label">The user-facing label for the preset.</param>
	/// <param name="Tooltip">The tooltip text for the preset.</param>
	private sealed record ColumnClassPreset(string Value, string Label, string Tooltip);

	/// <summary>
	/// A list of predefined column-class presets for the Calendar component, each with a value, label, and tooltip.
	/// </summary>
	private static readonly List<ColumnClassPreset> ColumnClassPresets =
	[
		new("", "(default)", "Use Calendar defaults: is-12-desktop is-12-widescreen is-6-fullhd is-4-4k."),
		new("is-12-desktop is-12-widescreen is-6-fullhd is-4-4k", "Conservative (1/1/2/3)", "1 column on desktop/widescreen, 2 on Full HD, 3 on 4K."),
		new("is-12-desktop is-6-widescreen is-6-fullhd is-4-4k", "Safe (1/2/2/3)", "1 column on desktop, 2 on widescreen/full HD, 3 on 4K."),
		new("is-12-desktop is-6-widescreen is-4-fullhd is-3-4k", "Balanced (1/2/3/4)", "1 column on desktop, 2 on widescreen, 3 on Full HD, 4 on 4K."),
		new("is-12", "Fixed Full Width", "Always full width (single column).")
	];

	private static readonly Dictionary<string, ColumnClassPreset> ColumnClassPresetLookup = ColumnClassPresets.ToDictionary(x => x.Value, x => x);

	private string SelectedColumnClass { get; set; } = ColumnClassPresets[3].Value;

	private DateOnly? LastMonthHeaderClicked;
	private DateOnly? LastWeekdayHeaderClicked;
	private int MonthHeaderClickCount;
	private int WeekdayHeaderClickCount;

	private static readonly List<string> ColorOptions =
	[
		"link",
		"info",
		"secondary",
		"tertiary",
		"highlight",
		"success",
		"warning",
		"danger"
	];

	private class Appointment
	{
		public string Color { get; init; } = ColorOptions[Random.Shared.Next(0, ColorOptions.Count)];
		public DateTime Date { get; set; }
		public string Title { get; set; } = "Meeting";
	}

	private void AddAppointment()
	{
		Appointments.Add(new Appointment
		{
			Date = SelectedDate,
			Title = $"Appointment {Appointments.Count + 1}"
		});

		if (Months.Any(x => x.Year == SelectedDate.Year && x.Month == SelectedDate.Month) == false)
			Months.Add(DateOnly.FromDateTime(SelectedDate));
	}

	private void AddRandomAppointment()
	{
		var first = new DateTime(SelectedDate.Year, SelectedDate.Month, 1, SelectedDate.Hour, SelectedDate.Minute, 0);
		var offset = Random.Shared.Next(0, DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month));
		var appointment = new Appointment { Date = first.AddDays(offset), Title = "Appointment" };

		Appointments.Add(appointment);

		if (Months.Any(x => x.Year == SelectedDate.Year && x.Month == SelectedDate.Month) == false)
			Months.Add(DateOnly.FromDateTime(SelectedDate));
	}

	private string GetTagClass(string color) => $"tag is-{color} mr-2 mb-2-tiny";
	private void ChangeMonth(bool increment) => SelectedDate = increment ? SelectedDate.AddMonths(1) : SelectedDate.AddMonths(-1);

	private void ClearAll()
	{
		Appointments.Clear();
		Months.Clear();
		Months.Add(DateTime.Today.ToDateOnly());
	}

	private Task HandleMonthHeaderClicked(DateOnly clickedMonth)
	{
		LastMonthHeaderClicked = clickedMonth;
		MonthHeaderClickCount++;
		return Task.CompletedTask;
	}

	private Task HandleWeekdayHeaderClicked(DateOnly clickedDay)
	{
		LastWeekdayHeaderClicked = clickedDay;
		WeekdayHeaderClickCount++;
		return Task.CompletedTask;
	}
}
