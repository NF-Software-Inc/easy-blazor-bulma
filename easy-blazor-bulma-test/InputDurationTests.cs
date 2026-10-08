using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
using System.Globalization;
using Xunit;

namespace easy_blazor_bulma_test;

public class InputDurationTests
{
    private const InputDurationOptions AllUnits =
        InputDurationOptions.ShowDays |
        InputDurationOptions.ShowHours |
        InputDurationOptions.ShowMinutes |
        InputDurationOptions.ShowSeconds |
        InputDurationOptions.ShowMilliseconds |
        InputDurationOptions.AllowGreaterThan24Hours |
        InputDurationOptions.ValidateTextInput;

    [Fact]
    public void FormatValueIncludesMilliseconds()
    {
        var input = new TestInputDuration(AllUnits);
        var value = new TimeSpan(days: 1, hours: 2, minutes: 3, seconds: 4, milliseconds: 5);

        var formatted = input.Format(value);

        Assert.Equal("1.02:03:04.005", formatted);
    }

    [Fact]
    public void FormattedValueWithMillisecondsCanBeParsed()
    {
        var input = new TestInputDuration(AllUnits);
        var expected = new TimeSpan(days: 1, hours: 2, minutes: 3, seconds: 4, milliseconds: 5);

        var parsed = input.TryParse("1.02:03:04.005", out var result, out var validationErrorMessage);

        Assert.True(parsed, validationErrorMessage);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ValueIsClampedWithMillisecondPrecision()
    {
        var input = new TestInputDuration(AllUnits & ~InputDurationOptions.AllowGreaterThan24Hours);

        var formatted = input.Format(TimeSpan.FromDays(1));

        Assert.Equal("0.23:59:59.999", formatted);
    }

    [Fact]
    public void TotalSecondsWithMillisecondsCanBeParsed()
    {
        var options =
            InputDurationOptions.ShowSeconds |
            InputDurationOptions.ShowMilliseconds |
            InputDurationOptions.DisplayMinutesAsSeconds |
            InputDurationOptions.AllowGreaterThan24Hours |
            InputDurationOptions.ValidateTextInput;

        var input = new TestInputDuration(options);
        var expected = TimeSpan.FromSeconds(90.125);

        var formatted = input.Format(expected);
        var parsed = input.TryParse(formatted, out var result, out var validationErrorMessage);

        Assert.Equal("90.125", formatted);
        Assert.True(parsed, validationErrorMessage);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void MillisecondsCanBeDisplayedAndParsedWithoutSeconds()
    {
        var options =
            InputDurationOptions.ShowMilliseconds |
            InputDurationOptions.AllowGreaterThan24Hours |
            InputDurationOptions.ValidateTextInput;

        var input = new TestInputDuration(options);
        var expected = TimeSpan.FromMilliseconds(125);

        var formatted = input.Format(expected);
        var parsed = input.TryParse(formatted, out var result, out var validationErrorMessage);

        Assert.Equal("125", formatted);
        Assert.True(parsed, validationErrorMessage);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(InputDurationOptions.ShowHours | InputDurationOptions.ShowMinutes | InputDurationOptions.ShowSeconds | InputDurationOptions.ShowMilliseconds | InputDurationOptions.DisplayDaysAsHours, "26:03:04.005")]
    [InlineData(InputDurationOptions.ShowMinutes | InputDurationOptions.ShowSeconds | InputDurationOptions.ShowMilliseconds | InputDurationOptions.DisplayHoursAsMinutes, "1563:04.005")]
    public void CustomUnitDisplaysPreserveMilliseconds(InputDurationOptions options, string expectedFormatted)
    {
        options |= InputDurationOptions.AllowGreaterThan24Hours | InputDurationOptions.ValidateTextInput;

        var input = new TestInputDuration(options);
        var expected = new TimeSpan(days: 1, hours: 2, minutes: 3, seconds: 4, milliseconds: 5);

        var formatted = input.Format(expected);
        var parsed = input.TryParse(formatted, out var result, out var validationErrorMessage);

        Assert.Equal(expectedFormatted, formatted);
        Assert.True(parsed, validationErrorMessage);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void DisplayFormatAndFormatterAreAppliedToDurationValues()
    {
        var value = new TimeSpan(days: 1, hours: 2, minutes: 3, seconds: 4, milliseconds: 5);
        var formatted = new TestInputDuration(AllUnits, "c", CultureInfo.InvariantCulture).Format(value);
        var customFormatted = new TestInputDuration(AllUnits, "c", formatter: _ => "custom").Format(value);

        Assert.Equal("1.02:03:04.0050000", formatted);
        Assert.Equal("custom", customFormatted);
    }

    [Fact]
    public void DurationUnitsCanBeEnteredDirectly()
    {
        var initialValue = new TimeSpan(days: 1, hours: 2, minutes: 3, seconds: 4, milliseconds: 5);
        var input = new TestInputDuration(InputDurationOptions.AllowGreaterThan24Hours);
        input.SetPopoutValue(initialValue);

        input.ChangeUnit(2, TimeSpan.TicksPerDay, 1);
        input.ChangeUnit(3, TimeSpan.TicksPerHour, 2);
        input.ChangeUnit(4, TimeSpan.TicksPerMinute, 3);
        input.ChangeUnit(5, TimeSpan.TicksPerSecond, 4);
        input.ChangeUnit(6, TimeSpan.TicksPerMillisecond, 5);

        Assert.Equal(new TimeSpan(days: 2, hours: 3, minutes: 4, seconds: 5, milliseconds: 6), input.GetPopoutValue());

        input = new TestInputDuration(InputDurationOptions.AllowNegative | InputDurationOptions.AllowGreaterThan24Hours);
        input.SetPopoutValue(-initialValue);
        input.ChangeUnit(5, TimeSpan.TicksPerSecond, 4);

        Assert.Equal(-initialValue.Add(TimeSpan.FromSeconds(1)), input.GetPopoutValue());
    }

    private sealed class TestInputDuration : InputDuration<TimeSpan>
    {
        public TestInputDuration(InputDurationOptions options, string? displayFormat = null, CultureInfo? culture = null, Func<TimeSpan, string?>? formatter = null)
        {
            Options = options;
            DisplayFormat = displayFormat;
            Culture = culture;
            Formatter = formatter;
        }

        public string Format(TimeSpan value) => FormatValueAsString(value);

        public bool TryParse(string value, out TimeSpan result, out string? validationErrorMessage) => TryParseValueFromString(value, out result, out validationErrorMessage);

        public void SetPopoutValue(TimeSpan value) => PopoutValue = value;

        public TimeSpan GetPopoutValue() => PopoutValue;

        public void ChangeUnit(int value, long ticksPerUnit, int currentValue) =>
            UpdateTimeUnit(new ChangeEventArgs { Value = value }, ticksPerUnit, currentValue);
    }
}
