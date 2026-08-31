using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// An input component for editing numeric values with custom formatting. Supported types are <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, <see cref="float"/>, <see cref="double"/>, and <see cref="decimal"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// <para>
/// The formatted value is displayed while the input does not have focus, the raw value is displayed when the input is focused to simplify editing.
/// </para>
///
/// <para>
/// There is 1 additional attribute that can be used: icon-class. It applies CSS classes to the resulting element as per its name.
/// </para>
///
/// <para>
/// <see href="https://bulma.io/documentation/form/general/">Bulma Documentation</see>
/// </para>
/// </remarks>
public partial class InputNumberFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue>
{
	/// <summary>
	/// An icon to display within the input.
	/// </summary>
	[Parameter]
	public string? Icon { get; set; } = "numbers";

	/// <summary>
	/// A standard or custom numeric format string to apply to the value when the input does not have focus.
	/// </summary>
	/// <remarks>
	/// <see href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Documentation</see>
	/// </remarks>
	[Parameter]
	public string? DisplayFormat { get; set; }

	/// <summary>
	/// An optional function to apply custom formatting to the value when the input does not have focus. Takes precedence over <see cref="DisplayFormat"/>.
	/// </summary>
	[Parameter]
	public Func<TValue?, string?>? Formatter { get; set; }

	/// <summary>
	/// The culture to use when formatting and parsing values.
	/// </summary>
	[Parameter]
	public CultureInfo? Culture { get; set; }

	/// <summary>
	/// Applies styles to the input according to the selected options.
	/// </summary>
	[Parameter]
	public InputStatus DisplayStatus { get; set; }

	/// <inheritdoc cref="InputDateTimeOptions.UseAutomaticStatusColors"/>
	[Parameter]
	public bool UseAutomaticStatusColors { get; set; } = true;

	/// <summary>
	/// Gets or sets the associated <see cref="ElementReference"/>.
	/// <para>
	/// May be <see langword="null"/> if accessed before the component is rendered.
	/// </para>
	/// </summary>
	[DisallowNull]
	public ElementReference? Element { get; private set; }

	private readonly string[] Filter = ["class", "icon-class"];

	private readonly Type UnderlyingType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
	private bool IsFocused;

	private CultureInfo FormatProvider => Culture ?? CultureInfo.CurrentCulture;

	private string MainCssClass
	{
		get
		{
			var css = "input";

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
	protected override void OnInitialized()
	{
		if (UnderlyingType != typeof(short) && UnderlyingType != typeof(int) && UnderlyingType != typeof(long) && UnderlyingType != typeof(float) && UnderlyingType != typeof(double) && UnderlyingType != typeof(decimal))
			throw new InvalidOperationException($"Unsupported type param '{UnderlyingType.Name}'. Must be of type short, int, long, float, double, or decimal.");
	}

	/// <inheritdoc />
	protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
		if (UseAutomaticStatusColors)
			ResetStatus();

		if (string.IsNullOrWhiteSpace(value))
		{
			result = default!;

			if (UseAutomaticStatusColors)
				DisplayStatus |= InputStatus.BackgroundSuccess;

			validationErrorMessage = null;
			return true;
		}
		else if (TryParseNumber(value, out result))
		{
			if (UseAutomaticStatusColors)
				DisplayStatus |= InputStatus.BackgroundSuccess;

			validationErrorMessage = null;
			return true;
		}
		else
		{
			result = default;

			if (UseAutomaticStatusColors)
				DisplayStatus |= InputStatus.BackgroundDanger;

			validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "The {0} field must be a number.", DisplayName ?? FieldIdentifier.FieldName);
			return false;
		}
	}

	/// <inheritdoc />
	protected override string FormatValueAsString(TValue? value)
	{
		if (value == null)
			return string.Empty;
		else if (IsFocused == false && Formatter != null)
			return Formatter(value) ?? string.Empty;
		else if (IsFocused == false && string.IsNullOrWhiteSpace(DisplayFormat) == false && value is IFormattable formattable)
			return formattable.ToString(DisplayFormat, FormatProvider);
		else if (value is IFormattable raw)
			return raw.ToString(null, FormatProvider);
		else
			return value.ToString() ?? string.Empty;
	}

	private bool TryParseNumber(string value, [MaybeNullWhen(false)] out TValue result)
	{
		const NumberStyles styles = NumberStyles.Any;
		var trimmed = value.Trim();

		if (UnderlyingType == typeof(short) && short.TryParse(trimmed, styles, FormatProvider, out var shortValue))
			result = (TValue)(object)shortValue;
		else if (UnderlyingType == typeof(int) && int.TryParse(trimmed, styles, FormatProvider, out var intValue))
			result = (TValue)(object)intValue;
		else if (UnderlyingType == typeof(long) && long.TryParse(trimmed, styles, FormatProvider, out var longValue))
			result = (TValue)(object)longValue;
		else if (UnderlyingType == typeof(float) && float.TryParse(trimmed, styles, FormatProvider, out var floatValue))
			result = (TValue)(object)floatValue;
		else if (UnderlyingType == typeof(double) && double.TryParse(trimmed, styles, FormatProvider, out var doubleValue))
			result = (TValue)(object)doubleValue;
		else if (UnderlyingType == typeof(decimal) && decimal.TryParse(trimmed, styles, FormatProvider, out var decimalValue))
			result = (TValue)(object)decimalValue;
		else
		{
			result = default;
			return false;
		}

		return true;
	}

	private void ResetStatus()
	{
		DisplayStatus &= ~InputStatus.BackgroundDanger;
		DisplayStatus &= ~InputStatus.BackgroundWarning;
		DisplayStatus &= ~InputStatus.BackgroundSuccess;
	}

	private void OnFocusIn()
	{
		IsFocused = true;
	}

	private void OnFocusOut()
	{
		IsFocused = false;
	}

	private void OnChange(ChangeEventArgs args)
	{
		CurrentValueAsString = args.Value?.ToString();
	}
}
