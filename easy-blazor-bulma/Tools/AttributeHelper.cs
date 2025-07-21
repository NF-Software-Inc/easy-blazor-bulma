using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// Contains methods to assist with parsing attribute values in components.
/// </summary>
public static class AttributeHelper
{
    /// <summary>
    /// Looks for the provided HTML attribute value and returns its value as a string.
    /// </summary>
    /// <param name="attributes">The collection containing HTML attribute data.</param>
    /// <param name="selector">The HTML attribute name to return.</param>
    public static string? GetValue(this IReadOnlyDictionary<string, object>? attributes, string selector)
    {
        if (attributes == null)
            return null;
        else if (attributes.TryGetValue(selector, out var value) && string.IsNullOrWhiteSpace(Convert.ToString(value, CultureInfo.InvariantCulture)) == false)
            return value.ToString();
        else
            return null;
    }

    /// <summary>
    /// Looks for the provided HTML attribute value and returns its value.
    /// </summary>
    /// <param name="attributes">The collection containing HTML attribute data.</param>
    /// <param name="selector">The HTML attribute name to return.</param>
    public static string? GetClass(this IReadOnlyDictionary<string, object>? attributes, string selector)
    {
        if (attributes == null)
            return null;
        else if (attributes.TryGetValue(selector, out var css) && string.IsNullOrWhiteSpace(Convert.ToString(css, CultureInfo.InvariantCulture)) == false)
            return $" {css}";
        else
            return null;
    }

    /// <summary>
    /// Returns all values in the provided collection except those with keys specified in the filter.
    /// </summary>
    /// <param name="attributes">The collection containing HTML attribute data.</param>
    /// <param name="filter">The items in the collection to ignore.</param>
    public static IReadOnlyDictionary<string, object>? GetFilteredAttributes(this IReadOnlyDictionary<string, object>? attributes, string[] filter)
    {
        if (attributes == null)
            return null;
        else if (filter.Length == 0)
            return attributes;
        else
            return attributes.Where(x => filter.Contains(x.Key) == false).ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// Returns values in the provided collection filtered as per the specified parameters.
    /// </summary>
    /// <param name="attributes">The collection containing HTML attribute data.</param>
    /// <param name="prefix">The text to match keys in the collection with.</param>
    /// <param name="matching">Specifies whether to return those that match the prefix or those that do not match.</param>
    public static IReadOnlyDictionary<string, object>? GetFilteredAttributes(this IReadOnlyDictionary<string, object>? attributes, string prefix, bool matching)
    {
        if (prefix.EndsWith('-') == false)
            prefix += '-';

        if (attributes == null)
            return null;
        else if (matching)
            return attributes.Where(x => x.Key.StartsWith(prefix)).ToDictionary(x => x.Key, x => x.Value);
        else
            return attributes.Where(x => x.Key.StartsWith(prefix) == false).ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// Returns all values in the provided collection and replaces any instances of readonly with disabled.
    /// </summary>
    /// <param name="attributes">The collection containing HTML attribute data.</param>
    public static IReadOnlyDictionary<string, object>? ConvertReadonlyToDisabled(this IReadOnlyDictionary<string, object>? attributes)
    {
        if (attributes == null)
            return null;
        else if (attributes.Any(x => x.Key == "readonly"))
            return attributes.Append(new KeyValuePair<string, object>("disabled", "disabled")).Where(x => x.Key != "readonly").ToDictionary(x => x.Key, x => x.Value);
        else
            return attributes;
    }

    /// <summary>
    /// Checks to see whether the provided collection contains any readonly or disabled attributes.
    /// </summary>
    /// <param name="attributes">The collection containing HTML attribute data.</param>
    public static bool IsDisabled(this IReadOnlyDictionary<string, object>? attributes) => attributes != null && attributes.Any(x => x.Key == "readonly" || (x.Key == "disabled" && (string.Equals(x.Value.ToString(), "disabled", StringComparison.OrdinalIgnoreCase) || string.Equals(x.Value.ToString(), "true", StringComparison.OrdinalIgnoreCase))));
}
