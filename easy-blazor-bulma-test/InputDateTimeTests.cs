using easy_blazor_bulma;
using System.Globalization;
using Xunit;

namespace easy_blazor_bulma_test;

public class InputDateTimeTests
{
    [Fact]
    public void DisplayFormatFormatsDateTime()
    {
        var input = new TestInputDateTime<DateTime> { DisplayFormat = "yyyy-MM-dd HH:mm:ss" };

        Assert.Equal("2026-02-08 03:15:43", input.Format(new DateTime(2026, 2, 8, 3, 15, 43)));
    }

    [Fact]
    public void DisplayFormatFormatsDateOnly()
    {
        var input = new TestInputDateTime<DateOnly> { DisplayFormat = "yyyy-MM-dd" };

        Assert.Equal("2026-02-08", input.Format(new DateOnly(2026, 2, 8)));
    }

    [Fact]
    public void DisplayFormatFormatsTimeOnly()
    {
        var input = new TestInputDateTime<TimeOnly> { DisplayFormat = "HH:mm:ss" };

        Assert.Equal("03:15:43", input.Format(new TimeOnly(3, 15, 43)));
    }

    [Fact]
    public void DisplayFormatFormatsTimeSpan()
    {
        var input = new TestInputDateTime<TimeSpan> { DisplayFormat = @"hh\:mm\:ss" };

        Assert.Equal("03:15:43", input.Format(new TimeSpan(3, 15, 43)));
    }

    [Fact]
    public void FormatterTakesPrecedenceOverDisplayFormat()
    {
        var input = new TestInputDateTime<DateTime>
        {
            DisplayFormat = "yyyy-MM-dd",
            Formatter = _ => "custom date"
        };

        Assert.Equal("custom date", input.Format(new DateTime(2026, 2, 8)));
    }

    [Fact]
    public void DisplayFormatUsesConfiguredCulture()
    {
        var input = new TestInputDateTime<DateTime>
        {
            DisplayFormat = "MMMM",
            Culture = CultureInfo.GetCultureInfo("fr-FR")
        };

        Assert.Equal("février", input.Format(new DateTime(2026, 2, 8)));
    }

    private sealed class TestInputDateTime<TValue> : InputDateTime<TValue>
    {
        public string Format(TValue value) => FormatValueAsString(value);
    }
}
