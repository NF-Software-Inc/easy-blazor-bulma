namespace easy_blazor_bulma.Tests;

using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;

public class ComponentDefaultCssClassTests
{
    [Fact]
    public void MediaObjectAppliesDefaultDimensionWhenNoneProvided()
    {
        var css = GetPrivateCssClass(typeof(MediaObject), "ImageCssClass", ("image-class", null));

        Assert.Contains("is-64x64", Tokens(css));
    }

    [Theory]
    [InlineData("is-128x128")]
    [InlineData("is-square")]
    public void MediaObjectSuppressesDefaultDimensionWhenProvided(string provided)
    {
        var css = GetPrivateCssClass(typeof(MediaObject), "ImageCssClass", ("image-class", provided));
        var tokens = Tokens(css);

        Assert.DoesNotContain("is-64x64", tokens);
        Assert.Contains(provided, tokens);
    }

    [Fact]
    public void TwoColumnsAppliesDefaultWidthsWhenNoneProvided()
    {
        var css = GetPrivateCssClass(typeof(TwoColumns), "LeftCssClass", ("left-class", null));

        Assert.Contains("is-4-tablet", Tokens(css));
    }

    [Fact]
    public void TwoColumnsSuppressesDefaultWidthsWhenProvided()
    {
        var css = GetPrivateCssClass(typeof(TwoColumns), "LeftCssClass", ("left-class", "is-6"));
        var tokens = Tokens(css);

        Assert.DoesNotContain("is-4-tablet", tokens);
        Assert.Contains("is-6", tokens);
        Assert.Contains("column", tokens);
    }

    [Fact]
    public void ThreeColumnsSuppressesDefaultWidthsWhenProvided()
    {
        var css = GetPrivateCssClass(typeof(ThreeColumns), "MiddleCssClass", ("middle-class", "is-half"));
        var tokens = Tokens(css);

        Assert.DoesNotContain("is-6-desktop", tokens);
        Assert.Contains("is-half", tokens);
    }

    [Fact]
    public void CalendarAppliesDefaultTableSizeWhenNoneProvided()
    {
        var css = GetPrivateCssClass(typeof(Calendar), "TableCssClass", ("table-class", null));
        var tokens = Tokens(css);

        Assert.Contains("is-size-7", tokens);
        Assert.Contains("is-bordered", tokens);
    }

    [Fact]
    public void CalendarSuppressesDefaultTableSizeWhenProvided()
    {
        var css = GetPrivateCssClass(typeof(Calendar), "TableCssClass", ("table-class", "is-size-5"));
        var tokens = Tokens(css);

        Assert.DoesNotContain("is-size-7", tokens);
        Assert.Contains("is-size-5", tokens);
        Assert.Contains("is-bordered", tokens);
    }

    private static string[] Tokens(string css) => css.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

    private static string GetPrivateCssClass(Type componentType, string propertyName, (string Key, string? Value) attribute)
    {
        var component = RuntimeHelpers.GetUninitializedObject(componentType);

        if (attribute.Value != null)
        {
            var attributes = new Dictionary<string, object> { { attribute.Key, attribute.Value } };
            var attributesProperty = componentType.GetProperty("AdditionalAttributes", BindingFlags.Instance | BindingFlags.Public);
            attributesProperty!.SetValue(component, attributes);
        }

        var property = componentType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);

        return (string)property!.GetValue(component)!;
    }
}
