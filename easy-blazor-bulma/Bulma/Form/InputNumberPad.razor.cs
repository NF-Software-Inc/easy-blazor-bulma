using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// An input component for entering numeric values. Supported types are <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, <see cref="float"/>, <see cref="double"/>, and <see cref="decimal"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// There are 3 additional attributes that can be used: columns-class, column-class, and button-class. Each of which apply CSS classes to the resulting elements as per their names.
/// </remarks>
public partial class InputNumberPad<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue>
{
    /// <summary>
    /// An icon to display within the input.
    /// </summary>
    [Parameter]
    public string? Icon { get; set; } = "numbers";

    /// <summary>
    /// Specifies whether to show a text input along with the number pad.
    /// </summary>
    [Parameter]
	public bool DisplayInput { get; set; }

	/// <summary>
	/// Specifies whether to round the number selection buttons.
	/// </summary>
	[Parameter]
	public bool IsRounded { get; set; } = true;

	/// <summary>
	/// Specifies whether to apply a border to the number selection buttons.
	/// </summary>
	[Parameter]
	public bool IsBordered { get; set; } = true;

	/// <summary>
	/// Applies styles to the input according to the selected options. Requires <see cref="DisplayInput"/> to be on.
	/// </summary>
	[Parameter]
	public InputStatus DisplayStatus { get; set; }

    /// <inheritdoc cref="InputDateTimeOptions.UseAutomaticStatusColors"/>
    [Parameter]
    public bool UseAutomaticStatusColors { get; set; } = true;

    /// <summary>
    /// Sets the text to display on a button below the number pad. Requires a value in <see cref="OnCustomButtonClicked"/>.
    /// </summary>
    [Parameter]
	public string? CustomButtonText { get; set; }

	/// <summary>
	/// A function that will run when the button is clicked. This will create a button below the number pad that displays <see cref="CustomButtonText"/>.
	/// </summary>
	[Parameter]
	public Func<Task>? OnCustomButtonClicked { get; set; }

	private readonly string[] Filter = new string[] { "class", "columns-class", "column-class", "button-class" };

	private readonly Type UnderlyingType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
	private bool IsNullable;

	private bool SupportsDecimals;
	private bool OnKeyDownPreventDefault;
	private readonly string[] DefaultKeys = new[] { "Escape", "Tab", "Enter", "NumpadEnter" };

	private string InternalValueAsString = string.Empty;

	private string MainCssClass
	{
		get
		{
			var css = "input mb-3";

			if (DisplayStatus.HasFlag(InputStatus.BackgroundDanger))
				css += " is-danger";
			else if (DisplayStatus.HasFlag(InputStatus.BackgroundWarning))
				css += " is-warning";
			else if (DisplayStatus.HasFlag(InputStatus.BackgroundSuccess))
				css += " is-success";

			return string.Join(' ', css, CssClass);
		}
	}

	private string ColumnsCssClass => string.Join(' ', "columns mb-0", AdditionalAttributes.GetClass("columns-class"));

	private string ColumnCssClass => string.Join(' ', "column pb-0", AdditionalAttributes.GetClass("column-class"));

	private string ButtonCssClass
	{
		get
		{
			var css = "button is-fullwidth mb-3";

            if (AdditionalAttributes.IsDisabled())
                css += " is-disabled";

            if (IsRounded)
				css += " is-rounded";

			if (IsBordered)
				css += " is-bordered";

			return string.Join(' ', css, AdditionalAttributes.GetClass("button-class"));
		}
	}

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		// Type checks
		IsNullable = Nullable.GetUnderlyingType(typeof(TValue)) != null;
		SupportsDecimals = UnderlyingType == typeof(float) || UnderlyingType == typeof(double) || UnderlyingType == typeof(decimal);

		if (UnderlyingType != typeof(short) && UnderlyingType != typeof(int) && UnderlyingType != typeof(long) && UnderlyingType != typeof(float) && UnderlyingType != typeof(double) && UnderlyingType != typeof(decimal))
			throw new InvalidOperationException($"Unsupported type param '{UnderlyingType.Name}'. Must be of type short, int, long, float, double, or decimal.");

		// Store initial value
		if (string.IsNullOrWhiteSpace(InternalValueAsString) && CurrentValue != null)
			InternalValueAsString = FormatValueAsString(CurrentValue);
	}

	/// <inheritdoc />
	protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
        if (UseAutomaticStatusColors)
            ResetStatus();

        if (IsNullable == false && string.IsNullOrWhiteSpace(value))
        {
            result = default!;

            if (UseAutomaticStatusColors)
                DisplayStatus |= InputStatus.BackgroundSuccess;

            validationErrorMessage = null;
            return true;
        }
        else if (BindConverter.TryConvertTo(value, CultureInfo.InvariantCulture, out result))
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

            validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "The {0} field must be a char.", DisplayName ?? FieldIdentifier.FieldName);
			return false;
		}
	}

	/// <inheritdoc />
	protected override string FormatValueAsString(TValue? value)
	{
		if (value is short shortValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || short.TryParse(InternalValueAsString, out short displayValue) && shortValue != displayValue)
				InternalValueAsString = shortValue.ToString();
		}
		else if (value is int intValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || int.TryParse(InternalValueAsString, out int displayValue) && intValue != displayValue)
				InternalValueAsString = intValue.ToString();
		}
		else if (value is long longValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || long.TryParse(InternalValueAsString, out long displayValue) && longValue != displayValue)
				InternalValueAsString = longValue.ToString();
		}
		else if (value is float floatValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || float.TryParse(InternalValueAsString, out float displayValue) && floatValue != displayValue)
				InternalValueAsString = floatValue.ToString();
		}
		else if (value is double doubleValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || double.TryParse(InternalValueAsString, out double displayValue) && doubleValue != displayValue)
				InternalValueAsString = doubleValue.ToString();
		}
		else if (value is decimal decimalValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || decimal.TryParse(InternalValueAsString, out decimal displayValue) && decimalValue != displayValue)
				InternalValueAsString = decimalValue.ToString();
		}

		return InternalValueAsString;
	}

	private void OnDigitClicked(int digit)
	{
        if (AdditionalAttributes.IsDisabled())
            return;

        InternalValueAsString += digit.ToString();

		if (InternalValueAsString.Length > 1 && InternalValueAsString[0] == '0' && InternalValueAsString[1] != '.')
			InternalValueAsString = InternalValueAsString.TrimStart('0');

		CurrentValueAsString = InternalValueAsString;
	}

	private void OnDecimalClicked()
	{
		if (InternalValueAsString.Contains('.'))
			return;

		if (InternalValueAsString.Length == 0)
			InternalValueAsString = "0.";
		else
			InternalValueAsString += '.';
	}

	private void OnBackspaceClicked()
	{
        if (AdditionalAttributes.IsDisabled())
            return;

        if (InternalValueAsString.Length == 0)
			return;

		if (IsNullable && InternalValueAsString.Length == 1)
		{
			InternalValueAsString = string.Empty;
			CurrentValueAsString = null;
		}
		else if (InternalValueAsString.Length == 1)
		{
			InternalValueAsString = "0";
			CurrentValueAsString = "0";
		}
		else
		{
			InternalValueAsString = InternalValueAsString[..^1];
			CurrentValueAsString = InternalValueAsString;
		}
	}

	private void OnResetClicked()
	{
        if (AdditionalAttributes.IsDisabled())
            return;

        if (IsNullable)
		{
			InternalValueAsString = string.Empty;
			CurrentValueAsString = null;
		}
		else
		{
			InternalValueAsString = "0";
			CurrentValueAsString = "0";
		}
	}

	private void OnInputKeyDown(KeyboardEventArgs args)
	{
		OnKeyDownPreventDefault = args.Code == "ArrowLeft" || args.Code == "ArrowRight" || DefaultKeys.Contains(args.Code) == false;

		if (args.Code != "ArrowDown" && args.Code != "ArrowUp" && args.Code != "ArrowLeft" && args.Code != "ArrowRight")
			OnKeyDown(args.Code);
	}

	private void OnButtonKeyDown(KeyboardEventArgs args)
	{
        if (AdditionalAttributes.IsDisabled())
            return;

        OnKeyDownPreventDefault = DefaultKeys.Contains(args.Code) == false;
		OnKeyDown(args.Code);
	}

	private void OnKeyDown(string key)
	{
        if (NumberKeys.TryGetValue(key, out int value))
			OnDigitClicked(value);
		else if (key == "Backspace")
			OnBackspaceClicked();
		else if (SupportsDecimals && (key == "NumpadDecimal" || key == "Period"))
			OnDecimalClicked();
	}

    private void ResetStatus()
    {
        DisplayStatus &= ~InputStatus.BackgroundDanger;
        DisplayStatus &= ~InputStatus.BackgroundWarning;
        DisplayStatus &= ~InputStatus.BackgroundSuccess;
    }

    private static readonly Dictionary<string, int> NumberKeys = new()
	{
		["Digit0"] = 0,
		["Digit1"] = 1,
		["Digit2"] = 2,
		["Digit3"] = 3,
		["Digit4"] = 4,
		["Digit5"] = 5,
		["Digit6"] = 6,
		["Digit7"] = 7,
		["Digit8"] = 8,
		["Digit9"] = 9,
		["Numpad0"] = 0,
		["Numpad1"] = 1,
		["Numpad2"] = 2,
		["Numpad3"] = 3,
		["Numpad4"] = 4,
		["Numpad5"] = 5,
		["Numpad6"] = 6,
		["Numpad7"] = 7,
		["Numpad8"] = 8,
		["Numpad9"] = 9
	};
}
