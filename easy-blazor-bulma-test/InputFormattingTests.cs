using easy_blazor_bulma;
using System.Globalization;
using Xunit;

namespace easy_blazor_bulma_test;

public class InputFormattingTests
{
    [Fact]
    public void NumberFormattedAppliesDisplayFormatWhenNotFocused()
    {
        var input = new TestInputNumberFormatted<decimal>("C2", CultureInfo.GetCultureInfo("en-US"));

        Assert.Equal("$1,234.50", input.Format(1234.5m));
    }

    [Fact]
    public void NumberFormattedUsesCustomFormatterWhenProvided()
    {
        var input = new TestInputNumberFormatted<int>("N0", formatter: x => $"{x} units");

        Assert.Equal("1234 units", input.Format(1234));
    }

    [Fact]
    public void NumberFormattedUsesCustomFormatterForNullValues()
    {
        var input = new TestInputNumberFormatted<double?>(formatter: x => x == null ? "Not set" : $"{x:N2} km");

        Assert.Equal("Not set", input.Format(null));
    }

    [Fact]
    public void NumberFormattedParsesFormattedValue()
    {
        var input = new TestInputNumberFormatted<decimal>("C2", CultureInfo.GetCultureInfo("en-US"));

        var parsed = input.TryParse("$1,234.50", out var result, out var validationErrorMessage);

        Assert.True(parsed, validationErrorMessage);
        Assert.Equal(1234.5m, result);
    }

    [Fact]
    public void NumberFormattedRejectsInvalidValue()
    {
        var input = new TestInputNumberFormatted<int>();

        var parsed = input.TryParse("abc", out _, out var validationErrorMessage);

        Assert.False(parsed);
        Assert.NotNull(validationErrorMessage);
    }

    [Fact]
    public void NumberFormattedThrowsForUnsupportedType()
    {
        var input = new TestInputNumberFormatted<string>();

        Assert.Throws<InvalidOperationException>(input.Initialize);
    }

    [Fact]
    public void ReadonlyAppliesDisplayFormat()
    {
        var input = new TestInputReadonly<DateTime>("yyyy-MM-dd", CultureInfo.InvariantCulture);

        Assert.Equal("2026-01-02", input.Format(new DateTime(2026, 1, 2, 3, 4, 5)));
    }

    [Fact]
    public void ReadonlyDisplaysNullText()
    {
        var input = new TestInputReadonly<int?>(nullText: "Not set");

        Assert.Equal("Not set", input.Format(null));
    }

    [Fact]
    public void ReadonlyKeepsCurrentValueWhenParsing()
    {
        var input = new TestInputReadonly<string>(value: "Original");

        var parsed = input.TryParse("Changed", out var result, out var validationErrorMessage);

        Assert.True(parsed, validationErrorMessage);
        Assert.Equal("Original", result);
    }

    [Fact]
    public void PhoneAppliesMaskToDigits()
    {
        var input = new TestInputPhone("(###) ###-####");

        Assert.Equal("(555) 123-4567", input.Format("5551234567"));
    }

    [Fact]
    public void PhoneLeavesValueWhenDigitCountDoesNotMatchMask()
    {
        var input = new TestInputPhone("(###) ###-####");

        Assert.Equal("12345", input.Format("12345"));
    }

    [Fact]
    public void PhoneFormatsValueWhenParsing()
    {
        var input = new TestInputPhone("###-###-####");

        var parsed = input.TryParse("(555) 123 4567", out var result, out var validationErrorMessage);

        Assert.True(parsed, validationErrorMessage);
        Assert.Equal("555-123-4567", result);
    }

    [Fact]
    public void PhoneRejectsValuesThatFailValidationExpression()
    {
        var input = new TestInputPhone(validationExpression: @"^\d{3}-\d{3}-\d{4}$", validationErrorMessage: "Invalid phone number.");

        var parsed = input.TryParse("555-1234", out _, out var validationErrorMessage);

        Assert.False(parsed);
        Assert.Equal("Invalid phone number.", validationErrorMessage);
    }

    [Fact]
    public void PhoneAcceptsValuesThatMatchValidationExpression()
    {
        var input = new TestInputPhone("###-###-####", @"^\d{3}-\d{3}-\d{4}$");

        var parsed = input.TryParse("5551234567", out var result, out var validationErrorMessage);

        Assert.True(parsed, validationErrorMessage);
        Assert.Equal("555-123-4567", result);
    }

    private sealed class TestInputNumberFormatted<TValue> : InputNumberFormatted<TValue>
    {
        public TestInputNumberFormatted(string? displayFormat = null, CultureInfo? culture = null, Func<TValue?, string?>? formatter = null)
        {
            DisplayFormat = displayFormat;
            Culture = culture;
            Formatter = formatter;
        }

        public string Format(TValue? value) => FormatValueAsString(value);

        public bool TryParse(string value, out TValue? result, out string? validationErrorMessage) => TryParseValueFromString(value, out result, out validationErrorMessage);

        public void Initialize() => OnInitialized();
    }

    private sealed class TestInputReadonly<TValue> : InputReadonly<TValue>
    {
        public TestInputReadonly(string? displayFormat = null, CultureInfo? culture = null, string? nullText = null, TValue? value = default)
        {
            DisplayFormat = displayFormat;
            Culture = culture;
            NullText = nullText;
            Value = value!;
        }

        public string Format(TValue? value) => FormatValueAsString(value);

        public bool TryParse(string value, out TValue? result, out string? validationErrorMessage) => TryParseValueFromString(value, out result, out validationErrorMessage);
    }

    private sealed class TestInputPhone : InputPhone<string>
    {
        public TestInputPhone(string? displayFormat = null, string? validationExpression = null, string? validationErrorMessage = null)
        {
            DisplayFormat = displayFormat;
            ValidationExpression = validationExpression;
            ValidationErrorMessage = validationErrorMessage;

            OnParametersSet();
        }

        public string Format(string? value) => FormatValueAsString(value);

        public bool TryParse(string value, out string? result, out string? validationErrorMessage) => TryParseValueFromString(value, out result, out validationErrorMessage);
    }
}
