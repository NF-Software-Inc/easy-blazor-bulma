using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
using System.Globalization;
using Xunit;

namespace easy_blazor_bulma_test;

public class InputDateTimeTests
{
    [Fact]
    public void DisplayFormatAndFormatterAreAppliedToDateTimeValues()
    {
        var value = new DateTime(2024, 2, 3, 4, 5, 6);
        var formatted = new TestInputDateTime("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).Format(value);
        var customFormatted = new TestInputDateTime("yyyy", formatter: _ => "custom").Format(value);

        Assert.Equal("2024-02-03 04:05:06", formatted);
        Assert.Equal("custom", customFormatted);
    }

    [Fact]
    public void TimeUnitsCanBeEnteredDirectly()
    {
        var initialValue = new DateTime(2024, 2, 3, 4, 5, 6);
        var input = new TestInputDateTime();
        input.SetPopoutValue(initialValue);

        input.ChangeTimeUnit(9, TimeSpan.TicksPerHour, 4, 23);

        Assert.Equal(initialValue.AddHours(5), input.GetPopoutValue());
    }

    private sealed class TestInputDateTime : InputDateTime<DateTime>
    {
        public TestInputDateTime(string? displayFormat = null, CultureInfo? culture = null, Func<DateTime, string?>? formatter = null)
        {
            Options = InputDateTimeOptions.None;
            DisplayFormat = displayFormat;
            Culture = culture;
            Formatter = formatter;
        }

        public string Format(DateTime value) => FormatValueAsString(value);

        public void SetPopoutValue(DateTime value) => PopoutValue = value;

        public DateTime GetPopoutValue() => PopoutValue;

        public void ChangeTimeUnit(int value, long ticksPerUnit, int currentValue, int maximum) =>
            UpdateTimeUnit(new ChangeEventArgs { Value = value }, ticksPerUnit, currentValue, maximum);
    }
}
