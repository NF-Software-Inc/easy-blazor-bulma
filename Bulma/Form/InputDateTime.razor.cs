﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// An input component for editing date and time values. Supported types are <see cref="DateTime"/>, <see cref="DateOnly"/>, <see cref="TimeSpan"/>, and <see cref="TimeOnly"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// There are 2 additional attributes that can be used: datetimepicker-class and icon-class. Each of which apply CSS classes to the resulting elements as per their names.
/// </remarks>
public partial class InputDateTime<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue>
{
	/// <summary>
	/// The number of hours to adjust by when the up or down arrows are clicked.
	/// </summary>
	[Parameter]
	[Range(1, 24)]
	public int StepHours { get; set; } = 1;

	/// <summary>
	/// The number of minutes to adjust by when the up or down arrows are clicked.
	/// </summary>
	[Parameter]
	[Range(1, 60)]
	public int StepMinutes { get; set; } = 5;

	/// <summary>
	/// The number of seconds to adjust by when the up or down arrows are clicked.
	/// </summary>
	[Parameter]
	[Range(1, 60)]
	public int StepSeconds { get; set; } = 15;

	/// <summary>
	/// Specifies the first day to display on the calendar view.
	/// </summary>
	[Parameter]
	public DayOfWeek StartOfWeek { get; set; } = DayOfWeek.Sunday;

	/// <summary>
	/// Specifies the number of years above and below the current year to display in the popout.
	/// </summary>
	[Parameter]
	public int PopoutYearRange { get; set; } = 12;

	/// <summary>
	/// An icon to display within the input.
	/// </summary>
	[Parameter]
	public string? Icon { get; set; } = "calendar_month";

	/// <summary>
	/// Applies styles to the input according to the selected options.
	/// </summary>
	[Parameter]
	public InputStatus DisplayStatus { get; set; }

	/// <summary>
	/// The configuration options to apply to the component.
	/// </summary>
	[Parameter]
	public InputDateTimeOptions Options { get; set; } =
		InputDateTimeOptions.ClickPopout |
		InputDateTimeOptions.PopoutBottom |
		InputDateTimeOptions.PopoutLeft |
		InputDateTimeOptions.ShowNowButton |
		InputDateTimeOptions.ShowResetButton |
		InputDateTimeOptions.UpdateOnPopoutChange |
		InputDateTimeOptions.UseAutomaticStatusColors |
		InputDateTimeOptions.ShowDate |
		InputDateTimeOptions.ShowHours |
		InputDateTimeOptions.ShowMinutes |
        InputDateTimeOptions.CloseOnDateClicked |
        InputDateTimeOptions.ValidateTextInput;

	[Inject]
	private IServiceProvider ServiceProvider { get; init; }

	private DateTime InitialValue;
	private DateTime PopoutValue;
	private bool IsPopoutDisplayed;
	private PopoutDisplayMode DisplayMode = PopoutDisplayMode.Calendar;

	private readonly bool IsNullable;
	private readonly Type UnderlyingType;
	private ElementReference? Element;
	private ILogger<InputDateTime<TValue>>? Logger;

	private bool Inactive => AdditionalAttributes != null && AdditionalAttributes.Any(x => x.Key == "readonly" || (x.Key == "disabled" && (x.Value.ToString() == "disabled" || x.Value.ToString() == "true")));

    private string FullCssClass
	{
		get
		{
			var css = "input";

			if (DisplayStatus.HasFlag(InputStatus.BackgroundDanger))
				css += " is-danger";
			else if (DisplayStatus.HasFlag(InputStatus.BackgroundWarning))
				css += " is-warning";
			else if (DisplayStatus.HasFlag(InputStatus.BackgroundSuccess))
				css += " is-success";

			return string.Join(' ', css, CssClass);
		}
	}

	private string TimePickerCssClass
	{
		get
		{
			var css = "datetimepicker";

			if (Options.HasFlag(InputDateTimeOptions.HoverPopout))
				css += " is-hoverable";

			if (IsPopoutDisplayed && Inactive == false)
				css += " is-active";

			if (Options.HasFlag(InputDateTimeOptions.PopoutBottom))
				css += " datetimepicker-below";
			else if (Options.HasFlag(InputDateTimeOptions.PopoutTop))
				css += " datetimepicker-above";

			if (Options.HasFlag(InputDateTimeOptions.PopoutLeft))
				css += " datetimepicker-left";
			else if (Options.HasFlag(InputDateTimeOptions.PopoutRight))
				css += " datetimepicker-right";

            if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("datetimepicker-class", out var additional) && string.IsNullOrWhiteSpace(Convert.ToString(additional, CultureInfo.InvariantCulture)) == false)
                css += $" {additional}";

            return css;
		}
	}

	private string IconCssClass
	{
		get
		{
			var css = "material-icons icon is-left";

			if (FullCssClass.Contains("is-small"))
				css += " is-small";

			if (DisplayStatus.HasFlag(InputStatus.IconDanger))
				css += " has-text-danger";
			else if (DisplayStatus.HasFlag(InputStatus.IconWarning))
				css += " has-text-warning";
			else if (DisplayStatus.HasFlag(InputStatus.IconSuccess))
				css += " has-text-success";

			if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("icon-class", out var additional) && string.IsNullOrWhiteSpace(Convert.ToString(additional, CultureInfo.InvariantCulture)) == false)
				css += $" {additional}";

            return css;
		}
	}

	private readonly string[] Filter = new string[] { "class", "datetimepicker-class", "icon-class" };
	private IReadOnlyDictionary<string, object>? FilteredAttributes => AdditionalAttributes?.Where(x => Filter.Contains(x.Key) == false).ToDictionary(x => x.Key, x => x.Value);

	private DateTime ValueAsDateTime
    {
        get
        {
            if (IsNullable && CurrentValue == null)
                return DateTime.Now;
            else if (UnderlyingType == typeof(DateTime))
                return (DateTime)Convert.ChangeType(CurrentValue!, typeof(DateTime));
            else if (UnderlyingType == typeof(DateOnly))
                return ((DateOnly)(object)CurrentValue!).ToDateTime(TimeOnly.MinValue);
            else if (UnderlyingType == typeof(TimeSpan))
                return DateTime.Today.Add((TimeSpan)(object)CurrentValue!);
            else
                return DateOnly.FromDateTime(DateTime.Today).ToDateTime((TimeOnly)(object)CurrentValue!);
        }
    }

    public InputDateTime()
	{
		var nullable = Nullable.GetUnderlyingType(typeof(TValue));

		UnderlyingType = nullable ?? typeof(TValue);
		IsNullable = nullable != null;

		if (UnderlyingType != typeof(DateTime) && UnderlyingType != typeof(DateOnly) && UnderlyingType != typeof(TimeSpan) && UnderlyingType != typeof(TimeOnly))
			throw new InvalidOperationException($"Unsupported type param '{UnderlyingType.Name}'. Must be of type {nameof(DateTime)}, {nameof(DateOnly)}, {nameof(TimeSpan)}, or {nameof(TimeOnly)}.");
	}

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		// Get services
		Logger = ServiceProvider.GetService<ILogger<InputDateTime<TValue>>>();

		// Validation
		if (UnderlyingType == typeof(TimeSpan) && Options.HasAnyFlag(InputDateTimeOptions.ShowHours | InputDateTimeOptions.ShowMinutes | InputDateTimeOptions.ShowSeconds) == false)
            Logger?.LogWarning($"Must set at least one of ShowHours, ShowMinutes, or ShowSeconds flags when using {nameof(TimeSpan)} for InputDateTime to work correctly.");

        if (UnderlyingType == typeof(TimeOnly) && Options.HasAnyFlag(InputDateTimeOptions.ShowHours | InputDateTimeOptions.ShowMinutes | InputDateTimeOptions.ShowSeconds) == false)
			Logger?.LogWarning($"Must set at least one of ShowHours, ShowMinutes, or ShowSeconds flags when using {nameof(TimeOnly)} for InputDateTime to work correctly.");

        // Set required options
        if (UnderlyingType == typeof(DateOnly))
			Options |= InputDateTimeOptions.ShowDate;

        // Unset invalid options
        if (UnderlyingType == typeof(DateOnly))
		{
			Options &= ~InputDateTimeOptions.ShowHours;
			Options &= ~InputDateTimeOptions.ShowMinutes;
			Options &= ~InputDateTimeOptions.ShowSeconds;
		}

        if (UnderlyingType == typeof(TimeSpan) || UnderlyingType == typeof(TimeOnly))
			Options &= ~InputDateTimeOptions.ShowDate;

		// Set starting values
		InitialValue = ValueAsDateTime;
        PopoutValue = ValueAsDateTime;
	}

    /// <inheritdoc />
    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            if (Element != null && AdditionalAttributes != null && AdditionalAttributes.TryGetValue("autofocus", out var _))
                await Element.Value.FocusAsync();
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
        // Validate
        if (Options.HasFlag(InputDateTimeOptions.UseAutomaticStatusColors))
            ResetStatus();

        if (string.IsNullOrWhiteSpace(value) == false && Options.HasFlag(InputDateTimeOptions.ValidateTextInput))
		{
            if (value.Count(x => x == '-') > 2 || value.Count(x => x == '/') > 2 || value.Count(x => x == ':') > 2)
            {
                result = default;

                if (Options.HasFlag(InputDateTimeOptions.UseAutomaticStatusColors))
                    DisplayStatus |= InputStatus.BackgroundDanger;

                validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "The '-', '/' and ':' characters may only appear twice in the {0} field.", DisplayName ?? FieldIdentifier.FieldName);
                return false;
            }
        }

		// Fix formatting
		if (string.IsNullOrWhiteSpace(value) == false)
		{
			if (Options.HasFlag(InputDateTimeOptions.ShowDate) == false && value.All(char.IsDigit))
                value = $"{value}:00:00";

            if (value.StartsWith(':'))
                value = $"0{value}";

            if (value.EndsWith(':'))
                value = $"{value}00";
        }

        // Try parse
        try
		{
            if (IsNullable == false && string.IsNullOrWhiteSpace(value))
            {
                result = default!;

                if (Options.HasFlag(InputDateTimeOptions.UseAutomaticStatusColors))
                    DisplayStatus |= InputStatus.BackgroundSuccess;

                validationErrorMessage = null;
                return true;
            }
            else if (BindConverter.TryConvertTo(value, CultureInfo.InvariantCulture, out result))
			{
                if (Options.HasFlag(InputDateTimeOptions.UseAutomaticStatusColors))
                    DisplayStatus |= InputStatus.BackgroundSuccess;

                validationErrorMessage = null;
                return true;
            }
			else
			{
                result = default;

                if (Options.HasFlag(InputDateTimeOptions.UseAutomaticStatusColors))
                    DisplayStatus |= InputStatus.BackgroundDanger;

                validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "The {0} field must be a date or time.", DisplayName ?? FieldIdentifier.FieldName);
                return false;
            }
        }
        catch (Exception e) when (e is FormatException || e is OverflowException)
		{
            result = default;

            if (Options.HasFlag(InputDateTimeOptions.UseAutomaticStatusColors))
                DisplayStatus |= InputStatus.BackgroundDanger;

            validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "The {0} could not be parsed as a date or time. Example: 2024-02-08 03:15:43", DisplayName ?? FieldIdentifier.FieldName);
            return false;
        }
	}

	/// <inheritdoc />
	protected override string FormatValueAsString(TValue? value) => value switch
	{
		DateTime dateTimeValue => FormatDateTime(dateTimeValue),
		DateOnly dateOnlyValue => FormatDateOnly(dateOnlyValue),
		TimeSpan timeSpanValue => FormatTimeSpan(timeSpanValue),
		TimeOnly timeOnlyValue => FormatTimeOnly(timeOnlyValue),
		_ => string.Empty
	};

	private string FormatDateTime(DateTime value)
	{
        var formatted = "";

		if (Options.HasFlag(InputDateTimeOptions.ShowDate))
			formatted += value.ToString("d") + ' ';

		formatted += (Options.HasFlag(InputDateTimeOptions.ShowHours), Options.HasFlag(InputDateTimeOptions.ShowMinutes), Options.HasFlag(InputDateTimeOptions.ShowSeconds)) switch
		{
			(true, true, true) => value.ToString("T"),
			(true, true, _) => value.ToString("t"),
			(true, _, _) => value.ToString("HH"),
			_ => ""
		};

        return formatted.TrimEnd(' ');
    }

	private string FormatDateOnly(DateOnly value) => FormatDateTime(value.ToDateTime(TimeOnly.MinValue));

	private string FormatTimeSpan(TimeSpan value) => FormatDateTime(DateTime.Today.Add(value));

	private string FormatTimeOnly(TimeOnly value) => FormatTimeSpan(value.ToTimeSpan());

	private void CheckKeyPress(KeyboardEventArgs args)
	{
		if (args.Code == "Escape" || args.Code == "Tab")
			ClosePopout();
	}

	private void OpenPopout()
	{
		if (IsPopoutDisplayed || Options.HasFlag(InputDateTimeOptions.NoPopout))
			return;

		IsPopoutDisplayed = true;

		if (Options.HasFlag(InputDateTimeOptions.UseAutomaticStatusColors))
			ResetStatus();
	}

	private void ClosePopout(bool save = false, bool reset = false, DateTime? value = null)
	{
		if (IsPopoutDisplayed == false || Options.HasFlag(InputDateTimeOptions.NoPopout))
			return;

		UpdateDisplayMode(PopoutDisplayMode.Calendar);
		IsPopoutDisplayed = false;

		if (value != null)
			PopoutValue = value.Value;

		if (reset)
			PopoutValue = InitialValue;

		if (save || reset)
			CurrentValueAsString = FormatDateTime(PopoutValue);
	}

	private void OnChange(ChangeEventArgs args)
	{
		CurrentValueAsString = args.Value?.ToString();
        PopoutValue = ValueAsDateTime;
    }

    private void OnYearSelected(int year)
    {
		UpdateDisplayMode(PopoutDisplayMode.Calendar);
		UpdatePopoutValue(year, PopoutValue.Month, PopoutValue.Day);
    }

    private void OnMonthSelected(int month)
    {
        UpdateDisplayMode(PopoutDisplayMode.Calendar);
        UpdatePopoutValue(PopoutValue.Year, month, PopoutValue.Day);
    }

    private void UpdateDisplayMode(PopoutDisplayMode displayMode)
	{
		IsPopoutDisplayed = true;
		DisplayMode = displayMode;
	}

    private void UpdatePopoutValue(TimeSpan adjustment)
	{
        PopoutValue = PopoutValue.Add(adjustment);

        if (Options.HasFlag(InputDateTimeOptions.UpdateOnPopoutChange))
		{
            if (UnderlyingType == typeof(DateTime))
                CurrentValueAsString = FormatDateTime(PopoutValue);
            else if (UnderlyingType == typeof(DateOnly))
                CurrentValueAsString = FormatDateOnly(DateOnly.FromDateTime(PopoutValue));
            else if (UnderlyingType == typeof(TimeSpan))
                CurrentValueAsString = FormatTimeSpan(PopoutValue.TimeOfDay);
            else
                CurrentValueAsString = FormatTimeOnly(TimeOnly.FromTimeSpan(PopoutValue.TimeOfDay));
        }
    }

	private void UpdatePopoutValue(int year, int month, int day)
	{
		if (day <= DateTime.DaysInMonth(year, month))
			PopoutValue = new DateTime(year, month, day).Add(PopoutValue.TimeOfDay);
		else
			PopoutValue = new DateTime(year, month, DateTime.DaysInMonth(year, month)).Add(PopoutValue.TimeOfDay);

        if (Options.HasFlag(InputDateTimeOptions.UpdateOnPopoutChange))
        {
            if (UnderlyingType == typeof(DateTime))
                CurrentValueAsString = FormatDateTime(PopoutValue);
            else if (UnderlyingType == typeof(DateOnly))
                CurrentValueAsString = FormatDateOnly(DateOnly.FromDateTime(PopoutValue));
            else if (UnderlyingType == typeof(TimeSpan))
                CurrentValueAsString = FormatTimeSpan(PopoutValue.TimeOfDay);
            else
                CurrentValueAsString = FormatTimeOnly(TimeOnly.FromTimeSpan(PopoutValue.TimeOfDay));
        }
    }

	private void UpdatePopoutValue(bool incrementMonth)
	{
		if (incrementMonth)
			PopoutValue = PopoutValue.AddMonths(1);
		else
            PopoutValue = PopoutValue.AddMonths(-1);

        if (Options.HasFlag(InputDateTimeOptions.UpdateOnPopoutChange))
        {
            if (UnderlyingType == typeof(DateTime))
                CurrentValueAsString = FormatDateTime(PopoutValue);
            else if (UnderlyingType == typeof(DateOnly))
                CurrentValueAsString = FormatDateOnly(DateOnly.FromDateTime(PopoutValue));
            else if (UnderlyingType == typeof(TimeSpan))
                CurrentValueAsString = FormatTimeSpan(PopoutValue.TimeOfDay);
            else
                CurrentValueAsString = FormatTimeOnly(TimeOnly.FromTimeSpan(PopoutValue.TimeOfDay));
        }
    }

	private void ResetStatus()
	{
		DisplayStatus &= ~InputStatus.BackgroundDanger;
		DisplayStatus &= ~InputStatus.BackgroundWarning;
		DisplayStatus &= ~InputStatus.BackgroundSuccess;
	}

    private IEnumerable<DateTime> GetCalendarDates()
	{
        var first = new DateTime(PopoutValue.Year, PopoutValue.Month, 1).AddDays(-1);

        if (first.DayOfWeek != StartOfWeek)
            first = first.GetPreviousWeekday(StartOfWeek);

        for (var day = first.Date; day.Date <= first.AddDays(41); day = day.AddDays(1))
            yield return day;
    }

    private IEnumerable<DateTime> GetCalendarMonths()
	{
        var first = new DateTime(DateTime.Today.Year, 1, 1);

        for (var day = first.Date; day.Date < first.AddYears(1); day = day.AddMonths(1))
            yield return day;
    }

    private enum PopoutDisplayMode
	{
		Calendar,
		Months,
		Years
	}
}
