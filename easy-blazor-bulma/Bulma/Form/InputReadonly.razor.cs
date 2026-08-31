using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// An input component to display a value that cannot be edited. Any type is supported.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// <para>
/// There is 1 additional attribute that can be used: icon-class. It applies CSS classes to the resulting element as per its name.
/// </para>
///
/// <para>
/// <see href="https://bulma.io/documentation/form/general/">Bulma Documentation</see>
/// </para>
/// </remarks>
public partial class InputReadonly<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue>
{
	/// <summary>
	/// An icon to display within the input.
	/// </summary>
	[Parameter]
	public string? Icon { get; set; }

	/// <summary>
	/// A standard or custom format string to apply to the value.
	/// </summary>
	/// <remarks>
	/// <see href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Documentation</see>
	/// </remarks>
	[Parameter]
	public string? DisplayFormat { get; set; }

	/// <summary>
	/// An optional function to apply custom formatting to the value. Takes precedence over <see cref="DisplayFormat"/>.
	/// </summary>
	[Parameter]
	public Func<TValue?, string?>? Formatter { get; set; }

	/// <summary>
	/// The culture to use when formatting values.
	/// </summary>
	[Parameter]
	public CultureInfo? Culture { get; set; }

	/// <summary>
	/// The text to display when the current value is null.
	/// </summary>
	[Parameter]
	public string? NullText { get; set; }

	/// <summary>
	/// Specifies whether to remove the input styling and display the value as text.
	/// </summary>
	[Parameter]
	public bool IsStatic { get; set; }

	/// <summary>
	/// Applies styles to the input according to the selected options.
	/// </summary>
	[Parameter]
	public InputStatus DisplayStatus { get; set; }

	/// <summary>
	/// Gets or sets the associated <see cref="ElementReference"/>.
	/// <para>
	/// May be <see langword="null"/> if accessed before the component is rendered.
	/// </para>
	/// </summary>
	[DisallowNull]
	public ElementReference? Element { get; private set; }

	private readonly string[] Filter = ["class", "icon-class", "readonly"];

	private CultureInfo FormatProvider => Culture ?? CultureInfo.CurrentCulture;

	private string MainCssClass
	{
		get
		{
			var css = "input";

			if (IsStatic)
				css += " is-static";

			if (DisplayStatus.HasFlag(InputStatus.BackgroundDanger))
				css += " is-danger";
			else if (DisplayStatus.HasFlag(InputStatus.BackgroundWarning))
				css += " is-warning";
			else if (DisplayStatus.HasFlag(InputStatus.BackgroundSuccess))
				css += " is-success";

			return string.Join(' ', css, CssClass);
		}
	}

	private string IconCssClass
	{
		get
		{
			var css = "material-icons icon is-left";

			if (MainCssClass.Contains("is-small"))
				css += " is-small";

			return string.Join(' ', css, AdditionalAttributes.GetValue("icon-class"));
		}
	}

	/// <inheritdoc />
	protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
		result = Value!;
		validationErrorMessage = null;

		return true;
	}

	/// <inheritdoc />
	protected override string FormatValueAsString(TValue? value)
	{
		if (Formatter != null)
			return Formatter(value) ?? string.Empty;
		else if (value == null)
			return NullText ?? string.Empty;
		else if (string.IsNullOrWhiteSpace(DisplayFormat) == false && value is IFormattable formattable)
			return formattable.ToString(DisplayFormat, FormatProvider);
		else if (value is IFormattable raw)
			return raw.ToString(null, FormatProvider);
		else
			return value.ToString() ?? string.Empty;
	}
}
