﻿using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace easy_blazor_bulma;

/// <summary>
/// Extension methods to assist with displaying values.
/// </summary>
internal static class InternalExtensions
{
    /// <summary>
    /// Returns the matching attribute from the specified value if found.
    /// </summary>
    /// <typeparam name="TModel">The datatype of the property to check.</typeparam>
    /// <typeparam name="TAttribute">The datatype of the attribute to return.</typeparam>
    /// <param name="value">The value to check.</param>
    internal static TAttribute? GetValueAttribute<TModel, TAttribute>(this TModel value) where TAttribute : Attribute
    {
        var memberName = value?.ToString();

        if (value == null || string.IsNullOrWhiteSpace(memberName))
            return null;

        var attribute = value.GetType()
            .GetMember(memberName)
            .First()
            .GetCustomAttribute<TAttribute>();

        return attribute;
    }

    /// <summary>
    /// Returns the <see cref="DisplayAttribute.Name"/> value of the selected flags in a CSV string.
    /// </summary>
    /// <typeparam name="TEnum">The type of the property.</typeparam>
    /// <param name="value">The value to find the name of.</param>
    /// <param name="ignoreZero">Specifies whether to ignore the flag with a zero value.</param>
    internal static string GetFlaggedEnumDisplay<TEnum>(this TEnum value, bool ignoreZero = true) where TEnum : Enum
    {
        var display = "";

        foreach (Enum flag in Enum.GetValues(value.GetType()))
        {
            if (ignoreZero && (int)(object)flag == 0)
                continue;
            else if (value.HasFlag(flag) == false)
                continue;

            var current = ((TEnum)flag).GetValueAttribute<TEnum, DisplayAttribute>()?.GetName();

            display += (string.IsNullOrWhiteSpace(current) == false ? current : flag.ToString()) + ' ';
        }

        return display.TrimEnd();
    }

    /// <summary>
    /// Returns the previous occurrence of the specified day of week or the current date if already on the specified day of week.
    /// </summary>
    /// <param name="date">The date to use as a base.</param>
    /// <param name="day">The day of the week requested.</param>
    internal static DateTime GetPreviousWeekday(this DateTime date, DayOfWeek day) => date.AddDays(-((date.DayOfWeek - day + 7) % 7));

	/// <summary>
	/// Splits the provided source into the specified number of collections.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source">The collection to split.</param>
	/// <param name="count">The number of collections to split the source into.</param>
	internal static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int count)
	{
		return source.Select((item, index) => new { index, item })
			.GroupBy(x => x.index % count)
			.Select(x => x.Select(y => y.item));
	}
}
