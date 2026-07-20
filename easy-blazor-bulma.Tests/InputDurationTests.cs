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
