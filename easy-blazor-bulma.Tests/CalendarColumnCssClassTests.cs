using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma.Tests;

public class CalendarColumnCssClassTests
{
	private static readonly string[] DefaultWidthClasses = ["is-12-desktop", "is-12-widescreen", "is-6-fullhd", "is-4-4k"];

	[Fact]
	public void ColumnCssClass_NoColumnClass_UsesDefaultWidths()
	{
		var sut = CreateCalendar(columnClass: null);
		var classes = GetColumnCssClassTokens(sut);

		Assert.Contains("column", classes);
		Assert.Contains("is-break-after", classes);
		Assert.Contains("is-fullwidth-print", classes);
		AssertDefaultWidthClassesPresent(classes);
	}

	[Fact]
	public void ColumnCssClass_NonWidthColumnClass_KeepsDefaultWidths()
	{
		var sut = CreateCalendar("is-primary mt-2 has-border");
		var classes = GetColumnCssClassTokens(sut);

		AssertDefaultWidthClassesPresent(classes);
		Assert.Contains("is-primary", classes);
		Assert.Contains("mt-2", classes);
		Assert.Contains("has-border", classes);
	}

	[Fact]
	public void ColumnCssClass_WidthColumnClass_SuppressesDefaultWidths()
	{
		var sut = CreateCalendar("is-6-desktop");
		var classes = GetColumnCssClassTokens(sut);

		AssertDefaultWidthClassesAbsent(classes);
		Assert.Contains("is-6-desktop", classes);
	}

	[Fact]
	public void ColumnCssClass_MixedWidthAndUtilityColumnClass_SuppressesDefaultsAndPreservesAllCustom()
	{
		var sut = CreateCalendar("is-4 has-border px-2");
		var classes = GetColumnCssClassTokens(sut);

		AssertDefaultWidthClassesAbsent(classes);
		Assert.Contains("is-4", classes);
		Assert.Contains("has-border", classes);
		Assert.Contains("px-2", classes);
	}

	private static Calendar CreateCalendar(string? columnClass)
	{
		var additionalAttributes = columnClass == null ? null : new Dictionary<string, object> { ["column-class"] = columnClass };

		return new Calendar
		{
			Months = [new DateOnly(2026, 1, 1), new DateOnly(2026, 2, 1)],
			Days = _ => builder => builder.AddMarkupContent(0, string.Empty),
			AdditionalAttributes = additionalAttributes
		};
	}

	private static HashSet<string> GetColumnCssClassTokens(Calendar calendar)
	{
		var property = typeof(Calendar).GetProperty("ColumnCssClass", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(property);

		var css = property.GetValue(calendar) as string ?? string.Empty;

		return css
			.Split([' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.ToHashSet(StringComparer.Ordinal);
	}

	private static void AssertDefaultWidthClassesPresent(ISet<string> classes)
	{
		foreach (var widthClass in DefaultWidthClasses)
			Assert.Contains(widthClass, classes);
	}

	private static void AssertDefaultWidthClassesAbsent(ISet<string> classes)
	{
		foreach (var widthClass in DefaultWidthClasses)
			Assert.DoesNotContain(widthClass, classes);
	}
}
