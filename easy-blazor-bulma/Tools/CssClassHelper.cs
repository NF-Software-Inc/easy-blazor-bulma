namespace easy_blazor_bulma;

/// <summary>
/// Contains methods to assist with detecting Bulma CSS classes so that default classes are only applied when a consumer has not provided an alternative.
/// </summary>
/// <remarks>
/// These checks avoid applying a default class when the consumer has supplied one from the same category, which would otherwise be ignored due to CSS cascade precedence.
/// </remarks>
public static class CssClassHelper
{
	/// <summary>
	/// Bulma color class tokens. Providing one of these should suppress a default color such as <c>is-primary</c> or <c>is-success</c>.
	/// </summary>
	public static readonly string[] ColorClasses =
	[
		"is-white",
		"is-black",
		"is-light",
		"is-dark",
		"is-primary",
		"is-link",
		"is-info",
		"is-success",
		"is-warning",
		"is-danger",
		"is-secondary",
		"is-tertiary",
		"is-highlight",
		"is-text",
		"is-ghost"
	];

	/// <summary>
	/// Bulma column width base tokens that should match either exactly (e.g. is-6) or as a responsive variant with a dash suffix (e.g. is-6-desktop).
	/// </summary>
	public static readonly string[] ColumnWidthClassPrefixes =
	[
		"is-0",
		"is-1",
		"is-2",
		"is-3",
		"is-4",
		"is-5",
		"is-6",
		"is-7",
		"is-8",
		"is-9",
		"is-10",
		"is-11",
		"is-12",
		"is-full",
		"is-half",
		"is-one-third",
		"is-two-thirds",
		"is-one-quarter",
		"is-three-quarters",
		"is-one-fifth",
		"is-two-fifths",
		"is-three-fifths",
		"is-four-fifths",
		"is-narrow"
	];

	/// <summary>
	/// Bulma font size class tokens. Providing one of these should suppress a default such as <c>is-size-7</c>.
	/// </summary>
	public static readonly string[] SizeClasses =
	[
		"is-size-1",
		"is-size-2",
		"is-size-3",
		"is-size-4",
		"is-size-5",
		"is-size-6",
		"is-size-7"
	];

	/// <summary>
	/// Bulma image dimension and aspect ratio class tokens. Providing one of these should suppress a default such as <c>is-64x64</c>.
	/// </summary>
	public static readonly string[] ImageDimensionClasses =
	[
		"is-16x16",
		"is-24x24",
		"is-32x32",
		"is-48x48",
		"is-64x64",
		"is-96x96",
		"is-128x128",
		"is-192x192",
		"is-256x256",
		"is-384x384",
		"is-512x512",
		"is-768x768",
		"is-1024x1024",
		"is-square",
		"is-1by1",
		"is-5by4",
		"is-4by3",
		"is-3by2",
		"is-5by3",
		"is-16by9",
		"is-2by1",
		"is-3by1",
		"is-4by5",
		"is-3by4",
		"is-2by3",
		"is-3by5",
		"is-9by16",
		"is-1by2",
		"is-1by3"
	];

	/// <summary>
	/// Determines whether the provided CSS class string contains a Bulma color class.
	/// </summary>
	/// <param name="css">The space separated CSS class string to inspect.</param>
	public static bool ContainsColorClass(string css) => ContainsExact(css, ColorClasses);

	/// <summary>
	/// Determines whether the provided CSS class string contains a Bulma column width class, including responsive variants.
	/// </summary>
	/// <param name="css">The space separated CSS class string to inspect.</param>
	public static bool ContainsColumnWidthClass(string css) => ContainsPrefixed(css, ColumnWidthClassPrefixes);

	/// <summary>
	/// Determines whether the provided CSS class string contains a Bulma font size class.
	/// </summary>
	/// <param name="css">The space separated CSS class string to inspect.</param>
	public static bool ContainsSizeClass(string css) => ContainsExact(css, SizeClasses);

	/// <summary>
	/// Determines whether the provided CSS class string contains a Bulma image dimension or aspect ratio class.
	/// </summary>
	/// <param name="css">The space separated CSS class string to inspect.</param>
	public static bool ContainsImageDimensionClass(string css) => ContainsExact(css, ImageDimensionClasses);

	private static bool ContainsExact(string css, string[] tokens) => css.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Any(tokens.Contains);

	private static bool ContainsPrefixed(string css, string[] prefixes) => css.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Any(x => prefixes.Any(y => x.StartsWith(y)));
}
