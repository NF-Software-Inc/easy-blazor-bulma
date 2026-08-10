namespace easy_blazor_bulma.Tests;

using System.Reflection;
using Xunit;

public class TitleBlockTests
{
    [Fact]
    public void DefaultColorIsAppliedWhenNoClassProvided()
    {
        var css = GetMainCssClass(null);

        Assert.Contains("is-primary", css.Split(' '));
    }

    [Fact]
    public void DefaultColorIsAppliedWhenNonColorClassProvided()
    {
        var css = GetMainCssClass("mb-4");

        Assert.Contains("is-primary", css.Split(' '));
        Assert.Contains("mb-4", css.Split(' '));
    }

    [Theory]
    [InlineData("is-dark")]
    [InlineData("is-info")]
    [InlineData("is-success mb-2")]
    [InlineData("is-warning")]
    public void DefaultColorIsSuppressedWhenColorClassProvided(string providedClass)
    {
        var css = GetMainCssClass(providedClass);
        var tokens = css.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

        Assert.DoesNotContain("is-primary", tokens);

        foreach (var token in providedClass.Split(' '))
            Assert.Contains(token, tokens);
    }

    [Fact]
    public void ExplicitPrimaryClassIsNotDuplicated()
    {
        var css = GetMainCssClass("is-primary");
        var count = css.Split(' ', System.StringSplitOptions.RemoveEmptyEntries).Count(x => x == "is-primary");

        Assert.Equal(1, count);
    }

    private static string GetMainCssClass(string? providedClass)
    {
        var component = new TitleBlock { Title = "Test" };

        if (providedClass != null)
            component.AdditionalAttributes = new Dictionary<string, object> { { "class", providedClass } };

        var property = typeof(TitleBlock).GetProperty("MainCssClass", BindingFlags.Instance | BindingFlags.NonPublic);

        return (string)property!.GetValue(component)!;
    }
}
