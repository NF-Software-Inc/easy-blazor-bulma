namespace easy_blazor_bulma.Tests;

using Xunit;

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
    [InlineData(
        InputDurationOptions.ShowHours |
        InputDurationOptions.ShowMinutes |
        InputDurationOptions.ShowSeconds |
        InputDurationOptions.ShowMilliseconds |
        InputDurationOptions.DisplayDaysAsHours,
        "26:03:04.005")]
    [InlineData(
        InputDurationOptions.ShowMinutes |
        InputDurationOptions.ShowSeconds |
        InputDurationOptions.ShowMilliseconds |
        InputDurationOptions.DisplayHoursAsMinutes,
        "1563:04.005")]
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

    private sealed class TestInputDuration : InputDuration<TimeSpan>
    {
        public TestInputDuration(InputDurationOptions options)
        {
            Options = options;
        }

        public string Format(TimeSpan value) => FormatValueAsString(value);

        public bool TryParse(string value, out TimeSpan result, out string? validationErrorMessage) =>
            TryParseValueFromString(value, out result, out validationErrorMessage);
    }
}
