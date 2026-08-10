namespace easy_blazor_bulma.Tests;

using Xunit;

public class CssClassHelperTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("mb-4 has-text-centered")]
    public void ContainsColorClassReturnsFalseWhenNoColorPresent(string? css)
    {
        Assert.False(CssClassHelper.ContainsColorClass(css));
    }

    [Theory]
    [InlineData("is-dark")]
    [InlineData("is-info")]
    [InlineData("mb-2 is-success")]
    [InlineData("is-warning mt-1")]
    [InlineData("is-primary")]
    public void ContainsColorClassReturnsTrueWhenColorPresent(string css)
    {
        Assert.True(CssClassHelper.ContainsColorClass(css));
    }

    [Fact]
    public void ContainsColorClassDoesNotMatchPartialToken()
    {
        Assert.False(CssClassHelper.ContainsColorClass("is-primary-ish"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("column has-text-centered")]
    [InlineData("is-fullwidth")]
    [InlineData("is-borderless")]
    public void ContainsColumnWidthClassReturnsFalseWhenNoWidthPresent(string? css)
    {
        Assert.False(CssClassHelper.ContainsColumnWidthClass(css));
    }

    [Theory]
    [InlineData("is-6")]
    [InlineData("is-6-desktop")]
    [InlineData("column is-half")]
    [InlineData("is-narrow")]
    [InlineData("is-12-widescreen")]
    public void ContainsColumnWidthClassReturnsTrueWhenWidthPresent(string css)
    {
        Assert.True(CssClassHelper.ContainsColumnWidthClass(css));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("has-text-weight-bold")]
    public void ContainsSizeClassReturnsFalseWhenNoSizePresent(string? css)
    {
        Assert.False(CssClassHelper.ContainsSizeClass(css));
    }

    [Theory]
    [InlineData("is-size-1")]
    [InlineData("mb-2 is-size-7")]
    public void ContainsSizeClassReturnsTrueWhenSizePresent(string css)
    {
        Assert.True(CssClassHelper.ContainsSizeClass(css));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("image")]
    public void ContainsImageDimensionClassReturnsFalseWhenNonePresent(string? css)
    {
        Assert.False(CssClassHelper.ContainsImageDimensionClass(css));
    }

    [Theory]
    [InlineData("is-128x128")]
    [InlineData("is-square")]
    [InlineData("image is-16by9")]
    public void ContainsImageDimensionClassReturnsTrueWhenPresent(string css)
    {
        Assert.True(CssClassHelper.ContainsImageDimensionClass(css));
    }
}
