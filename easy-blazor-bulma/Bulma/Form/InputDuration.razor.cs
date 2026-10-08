using easy_core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// An input component for editing duration values. Supported types are <see cref="TimeSpan"/> and <see cref="TimeOnly"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// There are 2 additional attributes that can be used: datetimepicker-class and icon-class. Each of which apply CSS classes to the resulting elements as per their names.
/// </remarks>
public partial class InputDuration<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue>
{
    /// <summary>
    /// The number of days to adjust by when the up or down arrows are clicked.
    /// </summary>
    [Parameter]
    [Range(1, 365)]
    public int StepDays { get; set; } = 1;

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
    /// The number of milliseconds to adjust by when the up or down arrows are clicked.
    /// </summary>
    [Parameter]
    [Range(1, 1_000)]
    public int StepMilliseconds { get; set; } = 100;

    /// <summary>
    /// An icon to display within the input.
    /// </summary>
    [Parameter]
    public string? Icon { get; set; } = "timer";

    /// <summary>
    /// An icon to reset the input.
    /// </summary>
    [Parameter]
    public string? ResetIcon { get; set; } = "close";

    /// <summary>
    /// Applies styles to the input according to the selected options.
    /// </summary>
    [Parameter]
    public InputStatus DisplayStatus { get; set; }

    /// <summary>
    /// A standard or custom format string used for the main textbox while it does not have focus.
    /// </summary>
    /// <remarks>
    /// <see href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Documentation</see>
    /// </remarks>
    [Parameter]
    public string? DisplayFormat { get; set; }

    /// <summary>
    /// An optional function to apply custom formatting to the main textbox while it does not have focus. Takes precedence over <see cref="DisplayFormat"/>.
    /// </summary>
    [Parameter]
    public Func<TValue?, string?>? Formatter { get; set; }

    /// <summary>
	/// The culture to use when formatting and parsing values.
	/// </summary>
	[Parameter]
    public CultureInfo? Culture { get; set; }

    /// <summary>
    /// The configuration options to apply to the component.
    /// </summary>
    [Parameter]
    public InputDurationOptions Options { get; set; } =
        InputDurationOptions.ClickPopout |
        InputDurationOptions.PopoutBottom |
        InputDurationOptions.PopoutLeft |
        InputDurationOptions.ShowResetButton |
        InputDurationOptions.UpdateOnPopoutChange |
        InputDurationOptions.UseAutomaticStatusColors |
        InputDurationOptions.ShowHours |
        InputDurationOptions.ShowMinutes |
        InputDurationOptions.ShowSeconds |
        InputDurationOptions.ValidateTextInput;

	/// <summary>
	/// Gets or sets the associated <see cref="ElementReference"/>.
	/// <para>
	/// May be <see langword="null"/> if accessed before the component is rendered.
	/// </para>
	/// </summary>
	[DisallowNull]
	public ElementReference? Element { get; private set; }

	[Inject]
	private IServiceProvider ServiceProvider { get; init; } = default!;

	private readonly string[] Filter = new string[] { "class", "datetimepicker-class", "icon-class" };

	private TimeSpan InitialValue;

    /// <summary>
    /// The value currently being edited in the popout. This is separate from the main input value to allow for canceling changes.
    /// </summary>
    private TimeSpan PopoutValue;
    private bool IsPopoutDisplayed;

    /// <summary>
    /// Indicates whether the main input is currently focused. Used to determine whether to apply formatting to the value.
    /// </summary>
    private bool IsMainInputFocused;
    private bool IsDaysInputFocused;
    private bool IsHoursInputFocused;
    private bool IsMinutesInputFocused;
    private bool IsSecondsInputFocused;
    private bool IsMillisecondsInputFocused;

    /// <summary>
    /// Number of minutes in the popout input. If the DisplayHoursAsMinutes option is set, this will return the total number of minutes, otherwise it will return the minutes component of the TimeSpan.
    /// </summary>
    private long PopoutMinutes => Options.HasFlag(InputDurationOptions.DisplayHoursAsMinutes)
        ? PopoutValue < TimeSpan.Zero ? Math.Abs((long)Math.Ceiling(PopoutValue.TotalMinutes)) : (long)Math.Floor(PopoutValue.TotalMinutes)
        : Math.Abs(PopoutValue.Minutes);

    /// <summary>
    /// Number of days in the popout input. This always return the days component of the TimeSpan.
    /// </summary>
    private int PopoutDays => Math.Abs(PopoutValue.Days);

    /// <summary>
    /// Number of hours in the popout input. If DisplayDaysAsHours, return the total number of hours, otherwise the hours component of the TimeSpan.
    /// </summary>
    private long PopoutHours => Options.HasFlag(InputDurationOptions.DisplayDaysAsHours)
        ? PopoutValue < TimeSpan.Zero ? Math.Abs((int)Math.Ceiling(PopoutValue.TotalHours)) : (int)Math.Floor(PopoutValue.TotalHours)
        : Math.Abs(PopoutValue.Hours);

    /// <summary>
    /// Number of seconds in the popout input. If DisplayMinutesAsSeconds, return the total number of seconds, otherwise the seconds component of the TimeSpan.
    /// </summary>
    private long PopoutSeconds => Options.HasFlag(InputDurationOptions.DisplayMinutesAsSeconds)
        ? PopoutValue < TimeSpan.Zero ? Math.Abs((int)Math.Ceiling(PopoutValue.TotalSeconds)) : (int)Math.Floor(PopoutValue.TotalSeconds)
        : Math.Abs(PopoutValue.Seconds);

    /// <summary>
    /// Number of milliseconds in the popout input. This always returns the milliseconds component of the TimeSpan.
    /// </summary>
    private int PopoutMilliseconds => Math.Abs(PopoutValue.Milliseconds);

    /// <summary>
    /// Gets the CSS class to apply to the timepicker input in the popout. Changes based on whether any of the unit inputs are focused.
    /// </summary>
    private string TimepickerInputCssClass => IsDaysInputFocused || IsHoursInputFocused || IsMinutesInputFocused || IsSecondsInputFocused || IsMillisecondsInputFocused ? "timepicker-input is-input" : "timepicker-input";

	private readonly Type UnderlyingType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
	private bool IsNullable;
	private ILogger<InputDuration<TValue>>? Logger;

    private int? MaximumHours => Options.HasFlag(InputDurationOptions.DisplayDaysAsHours) ? null : 23;
    private int? MaximumMinutes => Options.HasFlag(InputDurationOptions.DisplayHoursAsMinutes) ? null : 59;
    private int? MaximumSeconds => Options.HasFlag(InputDurationOptions.DisplayMinutesAsSeconds) ? null : 59;
    private const int MaximumMilliseconds = 999;

    /// <summary>
    /// Gets the culture to use for formatting and parsing values. Defaults to <see cref="CultureInfo.InvariantCulture"/> if <see cref="Culture"/> is not set.
    /// </summary>
    private CultureInfo FormatProvider => Culture ?? CultureInfo.InvariantCulture;

    private string MainCssClass
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

            if (Options.HasFlag(InputDurationOptions.HoverPopout))
                css += " is-hoverable";

            if (IsPopoutDisplayed && AdditionalAttributes.IsDisabled() == false)
                css += " is-active";

            if (Options.HasFlag(InputDurationOptions.PopoutBottom))
                css += " datetimepicker-below";
            else if (Options.HasFlag(InputDurationOptions.PopoutTop))
                css += " datetimepicker-above";

            if (Options.HasFlag(InputDurationOptions.PopoutLeft))
                css += " datetimepicker-left";
            else if (Options.HasFlag(InputDurationOptions.PopoutRight))
                css += " datetimepicker-right";

			return string.Join(' ', css, AdditionalAttributes.GetValue("datetimepicker-class"));
		}
    }

    private string IconCssClass
    {
        get
        {
            var css = "material-icons icon is-left";

            if (MainCssClass.Contains("is-small"))
                css += " is-small";

            if (DisplayStatus.HasFlag(InputStatus.IconDanger))
                css += " has-text-danger";
            else if (DisplayStatus.HasFlag(InputStatus.IconWarning))
                css += " has-text-warning";
            else if (DisplayStatus.HasFlag(InputStatus.IconSuccess))
                css += " has-text-success";

			return string.Join(' ', css, AdditionalAttributes.GetValue("icon-class"));
		}
    }

    private TimeSpan ValueAsTimeSpan
    {
        get
        {
            if (IsNullable && CurrentValue == null)
                return TimeSpan.Zero;
            else if (UnderlyingType == typeof(TimeSpan))
                return (TimeSpan)Convert.ChangeType(CurrentValue!, typeof(TimeSpan));
            else
                return ((TimeOnly)(object)CurrentValue!).ToTimeSpan();
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
		// Type checks
		IsNullable = Nullable.GetUnderlyingType(typeof(TValue)) != null;

		if (UnderlyingType != typeof(TimeSpan) && UnderlyingType != typeof(TimeOnly))
			throw new InvalidOperationException($"Unsupported type param '{UnderlyingType.Name}'. Must be of type {nameof(TimeSpan)} or {nameof(TimeOnly)}.");

		// Get services
		Logger = ServiceProvider.GetService<ILogger<InputDuration<TValue>>>();

		// Validation
		if (Options.HasAllFlags(InputDurationOptions.DisplayDaysAsHours | InputDurationOptions.ShowDays))
        {
            Options &= ~InputDurationOptions.DisplayDaysAsHours;
            Logger?.LogWarning("Cannot set both DisplayDaysAsHours and ShowDays for InputDuration.");
        }

        if (Options.HasAllFlags(InputDurationOptions.DisplayHoursAsMinutes | InputDurationOptions.ShowHours))
        {
            Options &= ~InputDurationOptions.DisplayHoursAsMinutes;
            Logger?.LogWarning("Cannot set both DisplayHoursAsMinutes and ShowHours for InputDuration.");
        }

        if (Options.HasAllFlags(InputDurationOptions.DisplayMinutesAsSeconds | InputDurationOptions.ShowMinutes))
        {
            Options &= ~InputDurationOptions.DisplayMinutesAsSeconds;
            Logger?.LogWarning("Cannot set both DisplayMinutesAsSeconds and ShowMinutes for InputDuration.");
        }

        if (Options.HasAllFlags(InputDurationOptions.DisplayDaysAsHours | InputDurationOptions.DisplayHoursAsMinutes))
        {
            Options &= ~InputDurationOptions.DisplayDaysAsHours;
            Logger?.LogWarning("Cannot set both DisplayDaysAsHours and DisplayHoursAsMinutes for InputDuration.");
        }

        if (Options.HasAllFlags(InputDurationOptions.DisplayDaysAsHours | InputDurationOptions.DisplayMinutesAsSeconds))
        {
            Options &= ~InputDurationOptions.DisplayDaysAsHours;
            Logger?.LogWarning("Cannot set both DisplayDaysAsHours and DisplayMinutesAsSeconds for InputDuration.");
        }

        if (Options.HasAllFlags(InputDurationOptions.DisplayHoursAsMinutes | InputDurationOptions.DisplayMinutesAsSeconds))
        {
            Options &= ~InputDurationOptions.DisplayHoursAsMinutes;
            Logger?.LogWarning("Cannot set both DisplayHoursAsMinutes and DisplayMinutesAsSeconds for InputDuration.");
        }

        if (Options.HasAnyFlag(InputDurationOptions.DisplayDaysAsHours | InputDurationOptions.DisplayHoursAsMinutes | InputDurationOptions.DisplayMinutesAsSeconds) && Options.HasFlag(InputDurationOptions.ConvertDecimals))
        {
            Options &= ~InputDurationOptions.ConvertDecimals;
            Logger?.LogWarning("Cannot combine ConvertDecimals with any of DisplayDaysAsHours, DisplayHoursAsMinutes, or DisplayMinutesAsSeconds for InputDuration.");
        }

        if (Options.HasFlag(InputDurationOptions.ShowMilliseconds) &&
            Options.HasFlag(InputDurationOptions.ShowSeconds) == false &&
            Options.HasAnyFlag(InputDurationOptions.ShowDays | InputDurationOptions.ShowHours | InputDurationOptions.ShowMinutes))
        {
            Options &= ~InputDurationOptions.ShowMilliseconds;
            Logger?.LogWarning("Cannot combine ShowMilliseconds with days, hours, or minutes without ShowSeconds for InputDuration.");
        }

        // Unset invalid options
        if (UnderlyingType == typeof(TimeOnly) && Options.HasAnyFlag(InputDurationOptions.AllowNegative | InputDurationOptions.AllowGreaterThan24Hours))
        {
            Options &= ~(InputDurationOptions.AllowNegative | InputDurationOptions.AllowGreaterThan24Hours);
            Logger?.LogWarning("Cannot set AllowNegative or AllowGreaterThan24Hours flags when using {type} with InputDuration.", nameof(TimeOnly));
        }

        // Set starting values
        InitialValue = ValueAsTimeSpan;
        PopoutValue = ValueAsTimeSpan;
    }

    /// <inheritdoc/>
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        // Validate
        if (Options.HasFlag(InputDurationOptions.UseAutomaticStatusColors))
            ResetStatus();

        var valid = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '.', ':' };

        if (string.IsNullOrWhiteSpace(value) == false && Options.HasFlag(InputDurationOptions.ValidateTextInput))
        {
            if (value.Any(x => valid.Contains(x) == false))
            {
                result = default;

                if (Options.HasFlag(InputDurationOptions.UseAutomaticStatusColors))
                    DisplayStatus |= InputStatus.BackgroundDanger;

                validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "The {0} field must contain only digits, '-', '.', and ':'.", DisplayName ?? FieldIdentifier.FieldName);
                return false;
            }

            if (Options.HasFlag(InputDurationOptions.AllowNegative) == false && value.Contains('-'))
            {
                result = default;

                if (Options.HasFlag(InputDurationOptions.UseAutomaticStatusColors))
                    DisplayStatus |= InputStatus.BackgroundDanger;

                validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "The {0} field does not have the AllowNegative option enabled.", DisplayName ?? FieldIdentifier.FieldName);
                return false;
            }

            var allowedPeriods = Options.HasAllFlags(InputDurationOptions.ShowDays | InputDurationOptions.ShowMilliseconds) &&
                Options.HasFlag(InputDurationOptions.DisplayDaysAsHours) == false ? 2 : 1;

            if (value.Count(x => x == '-') > 1 || value.Count(x => x == '.') > allowedPeriods || value.Count(x => x == ':') > 2)
            {
                result = default;

                if (Options.HasFlag(InputDurationOptions.UseAutomaticStatusColors))
                    DisplayStatus |= InputStatus.BackgroundDanger;

                validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "The {0} field contains too many separators.", DisplayName ?? FieldIdentifier.FieldName);
                return false;
            }

            if (value.Contains('-') && value.StartsWith('-') == false)
            {
                result = default;

                if (Options.HasFlag(InputDurationOptions.UseAutomaticStatusColors))
                    DisplayStatus |= InputStatus.BackgroundDanger;

                validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "The negative sign may only appear at the start of the {0} field.", DisplayName ?? FieldIdentifier.FieldName);
                return false;
            }

            if (Options.HasFlag(InputDurationOptions.DisplayMinutesAsSeconds) && Options.HasFlag(InputDurationOptions.ShowMilliseconds) == false && value.Contains('.'))
            {
                result = default;

                if (Options.HasFlag(InputDurationOptions.UseAutomaticStatusColors))
                    DisplayStatus |= InputStatus.BackgroundDanger;

                validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "Cannot enter decimal values when DisplayMinutesAsSeconds is active in the {0} field.", DisplayName ?? FieldIdentifier.FieldName);
                return false;
            }
        }

        // Fix formatting
        if (string.IsNullOrWhiteSpace(value) == false)
            value = FixStringFormatting(value);

        // Try parse
        try
        {
            if (string.IsNullOrWhiteSpace(value) == false && Options.HasFlag(InputDurationOptions.ConvertDecimals) && value.Count(x => x == '.') == 1)
            {
                var parts = value.Split('.');

                if (float.TryParse($"0.{parts[1]}", out var parsed) && (parsed * 60.0) < 59.5)
                    value = $"{parts[0]}:{((int)Math.Round(parsed * 60.0)).ToString().PadLeft(2, '0')}";
                else
                    throw new FormatException("Failed converting decimal to minutes or seconds.");
            }

            if (IsNullable == false && string.IsNullOrWhiteSpace(value))
            {
                result = default!;

                if (Options.HasFlag(InputDurationOptions.UseAutomaticStatusColors))
                    DisplayStatus |= InputStatus.BackgroundSuccess;

                validationErrorMessage = null;
                return true;
            }
            else if (BindConverter.TryConvertTo(value, CultureInfo.InvariantCulture, out result))
            {
                if (Options.HasFlag(InputDurationOptions.UseAutomaticStatusColors))
                    DisplayStatus |= InputStatus.BackgroundSuccess;

                validationErrorMessage = null;
                return true;
            }
            else
            {
                result = default;

                if (Options.HasFlag(InputDurationOptions.UseAutomaticStatusColors))
                    DisplayStatus |= InputStatus.BackgroundDanger;

                validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "The {0} field must be a time.", DisplayName ?? FieldIdentifier.FieldName);
                return false;
            }
        }
        catch (Exception e) when (e is FormatException || e is OverflowException)
        {
            result = default;

            if (Options.HasFlag(InputDurationOptions.UseAutomaticStatusColors))
                DisplayStatus |= InputStatus.BackgroundDanger;

            validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "The {0} could not be parsed as a time. Example: 1.03:15:43 = 1 day, 3 hours, 15 minutes, 43 seconds", DisplayName ?? FieldIdentifier.FieldName);
            return false;
        }
    }

    private string FixStringFormatting(string value)
    {
        // Invalid start
        var negative = value.StartsWith('-');

        if (negative)
            value = value.TrimStart('-');

        if (value.StartsWith('.') || value.StartsWith(':'))
            value = $"0{value}";

        // Invalid end
        if (value.EndsWith('.') || value.EndsWith(':'))
            value = $"{value}00";

        var showOnlyMilliseconds = Options.HasFlag(InputDurationOptions.ShowMilliseconds) &&
            Options.HasAnyFlag(InputDurationOptions.ShowDays | InputDurationOptions.ShowHours | InputDurationOptions.ShowMinutes | InputDurationOptions.ShowSeconds) == false;

        if (showOnlyMilliseconds)
        {
            var totalMilliseconds = Math.Abs(double.Parse(value, CultureInfo.InvariantCulture));
            value = TimeSpan.FromMilliseconds(totalMilliseconds).ToString("c", CultureInfo.InvariantCulture);

            return negative ? '-' + value : value;
        }

        // Decimal values
        if (value.Contains('.') && value.Contains(':') == false)
        {
            if (Options.HasFlag(InputDurationOptions.DisplayDaysAsHours))
            {
                var parts = value.Split('.');
                var partial = float.Parse($"0.{parts[1]}") * 60.0;
                var end = partial < 59.5 ? ((int)Math.Round(partial)).ToString().PadLeft(2, '0') : "59";

                value = $"{parts[0]}:{end}:00";
            }
            else if (Options.HasFlag(InputDurationOptions.DisplayHoursAsMinutes))
            {
                var parts = value.Split('.');
                var partial = float.Parse($"0.{parts[1]}") * 60.0;
                var end = partial < 59.5 ? ((int)Math.Round(partial)).ToString().PadLeft(2, '0') : "59";

                value = $"{parts[0]}:{end}";
            }
        }

        // Integral values
        if (value.Contains('.') == false && value.Contains(':') == false)
        {
            if (Options.HasFlag(InputDurationOptions.ShowDays))
                value = $"{value}.00:00:00";
            else if (Options.HasFlag(InputDurationOptions.ShowHours))
                value = $"{value}:00:00";
            else if (Options.HasFlag(InputDurationOptions.DisplayHoursAsMinutes))
                value = $"{value}:00";
            else if (Options.HasFlag(InputDurationOptions.ShowMinutes))
                value = $"00:{value}:00";
            else if (Options.HasFlag(InputDurationOptions.DisplayMinutesAsSeconds) == false && Options.HasFlag(InputDurationOptions.ShowSeconds))
                value = $"00:00:{value}";
        }

        // Custom display parsing
        if (Options.HasFlag(InputDurationOptions.DisplayDaysAsHours))
        {
            var parts = value.Split(':');
            var duration = TimeSpan.FromHours(Math.Abs(double.Parse(parts[0], CultureInfo.InvariantCulture)));

            if (parts.Length > 1)
                duration += TimeSpan.FromMinutes(double.Parse(parts[1], CultureInfo.InvariantCulture));

            if (parts.Length > 2)
                duration += TimeSpan.FromSeconds(double.Parse(parts[2], CultureInfo.InvariantCulture));

            value = duration.ToString("c", CultureInfo.InvariantCulture);
        }
        else if (Options.HasFlag(InputDurationOptions.DisplayHoursAsMinutes))
        {
            var parts = value.Split(':');
            var duration = TimeSpan.FromMinutes(Math.Abs(double.Parse(parts[0], CultureInfo.InvariantCulture)));

            if (parts.Length > 1)
                duration += TimeSpan.FromSeconds(double.Parse(parts[1], CultureInfo.InvariantCulture));

            value = duration.ToString("c", CultureInfo.InvariantCulture);
        }
        else if (Options.HasFlag(InputDurationOptions.DisplayMinutesAsSeconds))
        {
            var totalSeconds = Math.Abs(double.Parse(value, CultureInfo.InvariantCulture));
            value = TimeSpan.FromSeconds(totalSeconds).ToString("c", CultureInfo.InvariantCulture);
        }

        if (negative)
            return '-' + value;
        else
            return value;
    }

    /// <inheritdoc />
    protected override string FormatValueAsString(TValue? value)
    {
        if (IsMainInputFocused == false && Formatter != null)
            return Formatter(value) ?? string.Empty;
        else if (value == null)
            return string.Empty;
        else if (IsMainInputFocused == false && string.IsNullOrWhiteSpace(DisplayFormat) == false && value is IFormattable formattable)
            return formattable.ToString(DisplayFormat, FormatProvider);

        return value switch
        {
            TimeSpan timeSpanValue => FormatTimeSpan(timeSpanValue),
            TimeOnly timeOnlyValue => FormatTimeOnly(timeOnlyValue),
            _ => string.Empty
        };
    }

    private string FormatTimeSpan(TimeSpan value)
    {
        if (Options.HasFlag(InputDurationOptions.AllowNegative) == false && value < TimeSpan.Zero)
            value = TimeSpan.Zero;

        if (Options.HasFlag(InputDurationOptions.AllowGreaterThan24Hours) == false && value >= TimeSpan.FromDays(1))
            value = TimeSpan.FromDays(1).Add(Options.HasFlag(InputDurationOptions.ShowMilliseconds) ? TimeSpan.FromMilliseconds(-1) : TimeSpan.FromSeconds(-1));

        var formatted = "";
        var negative = value < TimeSpan.Zero;

        if (negative)
            formatted += '-';

        if (Options.HasFlag(InputDurationOptions.ShowDays))
            formatted += value.ToString("%d") + '.';

        if (Options.HasFlag(InputDurationOptions.ShowHours))
        {
            if (Options.HasFlag(InputDurationOptions.ShowDays))
                formatted += value.ToString("hh") + ':';
            else if (Options.HasFlag(InputDurationOptions.DisplayDaysAsHours))
                formatted += (negative ? Math.Abs((int)Math.Ceiling(value.TotalHours)) : (int)Math.Floor(value.TotalHours)).ToString() + ':';
            else
                formatted += value.ToString("%h") + ':';
        }

        if (Options.HasFlag(InputDurationOptions.ShowMinutes))
        {
            if (Options.HasFlag(InputDurationOptions.ShowHours))
                formatted += value.ToString("mm") + ':';
            else if (Options.HasFlag(InputDurationOptions.DisplayHoursAsMinutes))
                formatted += (negative ? Math.Abs((int)Math.Ceiling(value.TotalMinutes)) : (int)Math.Floor(value.TotalMinutes)).ToString() + ':';
            else
                formatted += value.ToString("%m") + ':';
        }

        if (Options.HasFlag(InputDurationOptions.ShowSeconds))
        {
            if (Options.HasFlag(InputDurationOptions.ShowMinutes))
                formatted += value.ToString("ss");
            else if (Options.HasFlag(InputDurationOptions.DisplayMinutesAsSeconds))
                formatted += (negative ? Math.Abs((int)Math.Ceiling(value.TotalSeconds)) : (int)Math.Floor(value.TotalSeconds)).ToString();
            else
                formatted += value.ToString("%s");
        }

        if (Options.HasFlag(InputDurationOptions.ShowMilliseconds))
        {
            if (Options.HasAnyFlag(InputDurationOptions.ShowDays | InputDurationOptions.ShowHours | InputDurationOptions.ShowMinutes | InputDurationOptions.ShowSeconds))
                formatted += '.';

            formatted += value.ToString("fff");
        }

        return formatted.TrimEnd('.', ':');
    }

    private string FormatTimeOnly(TimeOnly value) => FormatTimeSpan(value.ToTimeSpan());

    private void CheckKeyPress(KeyboardEventArgs args)
    {
        if (args.Code == "Escape" || args.Code == "Tab")
            ClosePopout();
    }

    private void OpenPopout()
    {
        if (IsPopoutDisplayed || Options.HasFlag(InputDurationOptions.NoPopout))
            return;

        IsPopoutDisplayed = true;

        if (Options.HasFlag(InputDurationOptions.UseAutomaticStatusColors))
            ResetStatus();
    }

    /// <summary>
    /// Handles the focus event for the main input. Sets the <see cref="IsMainInputFocused"/> property to true and opens the popout if applicable.
    /// </summary>
    private void OnMainInputFocus()
    {
        IsMainInputFocused = true;
        OpenPopout();
    }

    /// <summary>
    /// Handles the blur event for the main input. Sets the <see cref="IsMainInputFocused"/> property to false.
    /// </summary>
    private void OnMainInputFocusOut() => IsMainInputFocused = false;

    private void ClosePopout(bool save = false, bool reset = false, bool clear = false)
    {
        if ((IsPopoutDisplayed == false && Options.HasFlag(InputDurationOptions.HoverPopout) == false) || Options.HasFlag(InputDurationOptions.NoPopout))
            return;

        IsPopoutDisplayed = false;

        if (reset)
            PopoutValue = InitialValue;

		if (clear)
			PopoutValue = default;

		if (save || reset || clear)
            CurrentValueAsString = FormatTimeSpan(PopoutValue);
    }

    private void ResetButtonHandler()
    {
        CurrentValueAsString = FormatTimeSpan(InitialValue);
    }

    private void OnChange(ChangeEventArgs args)
    {
        CurrentValueAsString = args.Value?.ToString();
        PopoutValue = ValueAsTimeSpan;
    }

    private void UpdatePopoutValue(TimeSpan adjustment)
    {
        var adjusted = PopoutValue.Add(adjustment);

        if (Options.HasFlag(InputDurationOptions.AllowNegative) == false && adjusted < TimeSpan.Zero)
            PopoutValue = TimeSpan.Zero;
        else if (Options.HasFlag(InputDurationOptions.AllowGreaterThan24Hours) == false && adjusted >= TimeSpan.FromDays(1))
            PopoutValue = TimeSpan.FromDays(1).Add(Options.HasFlag(InputDurationOptions.ShowMilliseconds) ? TimeSpan.FromMilliseconds(-1) : TimeSpan.FromSeconds(-1));
        else
            PopoutValue = adjusted;

        if (Options.HasFlag(InputDurationOptions.UpdateOnPopoutChange))
            CurrentValueAsString = FormatTimeSpan(PopoutValue);
    }

    private void OnDaysInputFocusIn() => IsDaysInputFocused = true;

    private void OnDaysInputFocusOut() => IsDaysInputFocused = false;

    private void OnHoursInputFocusIn() => IsHoursInputFocused = true;

    private void OnHoursInputFocusOut() => IsHoursInputFocused = false;

    private void OnMinutesInputFocusIn() => IsMinutesInputFocused = true;

    private void OnMinutesInputFocusOut() => IsMinutesInputFocused = false;

    private void OnSecondsInputFocusIn() => IsSecondsInputFocused = true;

    private void OnSecondsInputFocusOut() => IsSecondsInputFocused = false;

    private void OnMillisecondsInputFocusIn() => IsMillisecondsInputFocused = true;

    private void OnMillisecondsInputFocusOut() => IsMillisecondsInputFocused = false;

    private void OnPopoutDaysChanged(ChangeEventArgs args) =>
        UpdatePopoutUnit(args, PopoutDays, null, TimeSpan.TicksPerDay);

    private void OnPopoutMinutesChanged(ChangeEventArgs args) =>
        UpdatePopoutUnit(args, PopoutMinutes, MaximumMinutes, TimeSpan.TicksPerMinute);

    private void OnPopoutHoursChanged(ChangeEventArgs args) =>
        UpdatePopoutUnit(args, PopoutHours, MaximumHours, TimeSpan.TicksPerHour);

    private void OnPopoutSecondsChanged(ChangeEventArgs args) =>
        UpdatePopoutUnit(args, PopoutSeconds, MaximumSeconds, TimeSpan.TicksPerSecond);

    private void OnPopoutMillisecondsChanged(ChangeEventArgs args) =>
        UpdatePopoutUnit(args, PopoutMilliseconds, MaximumMilliseconds, TimeSpan.TicksPerMillisecond);

    /// <summary>
    /// Handles the change event for a unit input in the popout. Validates and updates the <see cref="PopoutValue"/> based on the new unit value, ensuring it adheres to the configured options.
    /// </summary>
    /// <param name="args">The change event arguments containing the new unit value.</param>
    /// <param name="displayedUnits">The current value of the unit before the change.</param>
    /// <param name="maxUnits">The maximum allowed value for the unit.</param>
    /// <param name="ticksPerUnit">The number of ticks per unit.</param>
    private void UpdatePopoutUnit(ChangeEventArgs args, long displayedUnits, int? maxUnits, long ticksPerUnit)
    {
        // Parse the new unit value from the input. If parsing fails, do not update the PopoutValue.
        if (int.TryParse(args.Value?.ToString(), NumberStyles.None, FormatProvider, out var units) == false)
            return;

        // Adjust only the selected unit, preserving the sign and all untouched component ticks
        var allowedUnits = Math.Min(units, maxUnits ?? TimeSpan.MaxValue.Ticks / ticksPerUnit);
        var adjustment = TimeSpan.FromTicks((allowedUnits - displayedUnits) * ticksPerUnit);
        UpdatePopoutValue(PopoutValue < TimeSpan.Zero ? -adjustment : adjustment);
    }

    private void ResetStatus()
    {
        DisplayStatus &= ~InputStatus.BackgroundDanger;
        DisplayStatus &= ~InputStatus.BackgroundWarning;
        DisplayStatus &= ~InputStatus.BackgroundSuccess;
    }
}
