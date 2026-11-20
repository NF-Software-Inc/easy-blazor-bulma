using easy_core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// An input component for entering numeric values. Supported types are <see cref="sbyte"/>, <see cref="byte"/>, <see cref="short"/>, <see cref="ushort"/>, <see cref="int"/>, <see cref="uint"/>, <see cref="long"/>, <see cref="ulong"/>, <see cref="float"/>, <see cref="double"/>, and <see cref="decimal"/>.
/// </summary>
/// <typeparam name="TValue">The bound numeric value type. Nullable forms of the supported numeric types are also accepted.</typeparam>
/// <remarks>
/// <para><b>Additional attributes for custom styling:</b></para>
/// <list type="bullet">
///   <item><term><c>columns-class</c></term><description>Classes added to the outer keypad columns container.</description></item>
///   <item><term><c>column-class</c></term><description>Classes added to each keypad column.</description></item>
///   <item><term><c>button-class</c></term><description>Classes added to each keypad and custom-action button.</description></item>
/// </list>
/// </remarks>
public partial class InputNumberPad<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue>, IAsyncDisposable
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
    /// The culture used to format and parse numeric values and determine the decimal separator. Defaults to <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    [Parameter]
    public CultureInfo? Culture { get; set; }

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

	/// <summary>
	/// Specifies whether the arrow keys move focus between the display input, keypad buttons, and custom button.
	/// </summary>
	/// <remarks>
	/// Set to <see langword="false"/> when embedding the component inside a container, such as a table or grid, that handles arrow-key focus navigation. Digit, decimal, and Backspace key entry remain active when disabled.
	/// </remarks>
	[Parameter]
	public bool UseArrowKeyNavigation { get; set; } = true;

	/// <summary>
	/// Gets or sets the associated <see cref="ElementReference"/>.
	/// <para>
	/// May be <see langword="null"/> if accessed before the component is rendered.
	/// </para>
	/// </summary>
	[DisallowNull]
	public ElementReference? Element { get; private set; }

	[Inject]
	private IJSRuntime JsRuntime { get; init; } = default!;

	private const int KeypadRows = 4;
	private const int KeypadColumns = 3;

	private readonly string[] Filter = ["class", "id", "columns-class", "column-class", "button-class"];

	private readonly Type UnderlyingType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
	private bool IsNullable;

	private CultureInfo DisplayProvider => Culture ?? CultureInfo.CurrentCulture;
	private CultureInfo FormatProvider => Culture ?? CultureInfo.InvariantCulture;
	private string DecimalSeparator => FormatProvider.NumberFormat.NumberDecimalSeparator;

	private bool SupportsDecimals;
	private bool WasDisplayInput;
	private bool RegisterInputPreventDefault;
	private bool? RegisteredUseArrowKeyNavigation;
	private int LastColumnIndex;

	private ElementReference? KeypadElement;
	private ElementReference? CustomButtonElement;
	private static readonly string[] PassthroughKeys = ["Escape", "Tab", "Enter"];
	private static readonly string[] ArrowKeys = ["ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight"];
	private static readonly string[] DefaultKeys = [.. PassthroughKeys, .. ArrowKeys];
	private static readonly Dictionary<string, int> NumberKeys = new()
	{
		["0"] = 0,
		["1"] = 1,
		["2"] = 2,
		["3"] = 3,
		["4"] = 4,
		["5"] = 5,
		["6"] = 6,
		["7"] = 7,
		["8"] = 8,
		["9"] = 9
	};
	private string[] AllowedKeys => UseArrowKeyNavigation ? PassthroughKeys : DefaultKeys;

	private string InternalValueAsString = string.Empty;

	private readonly string ButtonId = Guid.NewGuid().ToHtmlId().ToString("N");
	private string InputId => AdditionalAttributes.GetValue("id") ?? $"{ButtonId}-input";
	private string GetButtonId(int row, int column) => string.Create(CultureInfo.InvariantCulture, $"{ButtonId}-{row}-{column}");

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
	private string ColumnsCssClass => string.Join(' ', "columns mb-0", AdditionalAttributes.GetValue("columns-class"));
	private string ColumnCssClass => string.Join(' ', "column pb-0", AdditionalAttributes.GetValue("column-class"));
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

			return string.Join(' ', css, AdditionalAttributes.GetValue("button-class"));
		}
	}

	private static readonly Type[] SupportedTypes =
	[
		typeof(sbyte),
		typeof(byte),
		typeof(short),
		typeof(ushort),
		typeof(int),
		typeof(uint),
		typeof(long),
		typeof(ulong),
		typeof(float),
		typeof(double),
		typeof(decimal)
	];

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		// Type checks
		IsNullable = Nullable.GetUnderlyingType(typeof(TValue)) != null;
		SupportsDecimals = UnderlyingType == typeof(float) || UnderlyingType == typeof(double) || UnderlyingType == typeof(decimal);

		if (SupportedTypes.Contains(UnderlyingType) == false)
			throw new InvalidOperationException($"Unsupported type param '{UnderlyingType.Name}'. Must be of type short, int, long, ushort, uint, ulong, float, double, or decimal.");

		// Store initial value
		if (string.IsNullOrWhiteSpace(InternalValueAsString) && CurrentValue != null)
			InternalValueAsString = FormatValueAsString(CurrentValue);
	}

	/// <inheritdoc />
	protected override void OnParametersSet()
	{
		base.OnParametersSet();

		if (DisplayInput && WasDisplayInput == false)
			RegisterInputPreventDefault = true;

		WasDisplayInput = DisplayInput;
	}

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);

		if (KeypadElement != null && KeypadElement.Value.Id != null && RegisteredUseArrowKeyNavigation != UseArrowKeyNavigation)
		{
			var allowed = AllowedKeys.Append(" ").ToArray();

			await JsRuntime.RegisterKeyDownPreventDefault(KeypadElement.Value, allowed, false, "button");

			if (CustomButtonElement != null && CustomButtonElement.Value.Id != null)
				await JsRuntime.RegisterKeyDownPreventDefault(CustomButtonElement.Value, allowed, false, "button");

			if (DisplayInput)
				RegisterInputPreventDefault = true;

			RegisteredUseArrowKeyNavigation = UseArrowKeyNavigation;
		}

		if (RegisterInputPreventDefault && DisplayInput && Element != null && Element.Value.Id != null)
		{
			RegisterInputPreventDefault = false;
			await JsRuntime.RegisterKeyDownPreventDefault(Element.Value, AllowedKeys, false);
		}
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		try
		{
			if (Element != null && Element.Value.Id != null)
				await JsRuntime.UnregisterKeyDownPreventDefault(Element.Value);

			if (KeypadElement != null && KeypadElement.Value.Id != null)
				await JsRuntime.UnregisterKeyDownPreventDefault(KeypadElement.Value);

			if (CustomButtonElement != null && CustomButtonElement.Value.Id != null)
				await JsRuntime.UnregisterKeyDownPreventDefault(CustomButtonElement.Value);
		}
		catch (Exception e) when (e is JSDisconnectedException or TaskCanceledException or ObjectDisposedException) { }

		((IDisposable)this).Dispose();
		GC.SuppressFinalize(this);
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
		else if (BindConverter.TryConvertTo(value, FormatProvider, out result))
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

			validationErrorMessage = string.Format(DisplayProvider, "The {0} field must be a number.", DisplayName ?? FieldIdentifier.FieldName);
			return false;
		}
	}

	/// <inheritdoc />
	protected override string FormatValueAsString(TValue? value)
	{
		if (value is sbyte sbyteValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || sbyte.TryParse(InternalValueAsString, FormatProvider, out sbyte displayValue) == false || sbyteValue != displayValue)
				InternalValueAsString = sbyteValue.ToString(FormatProvider);
		}
		else if (value is byte byteValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || byte.TryParse(InternalValueAsString, FormatProvider, out byte displayValue) == false || byteValue != displayValue)
				InternalValueAsString = byteValue.ToString(FormatProvider);
		}
		else if (value is short shortValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || short.TryParse(InternalValueAsString, FormatProvider, out short displayValue) == false || shortValue != displayValue)
				InternalValueAsString = shortValue.ToString(FormatProvider);
		}
		else if (value is ushort ushortValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || ushort.TryParse(InternalValueAsString, FormatProvider, out ushort displayValue) == false || ushortValue != displayValue)
				InternalValueAsString = ushortValue.ToString(FormatProvider);
		}
		else if (value is int intValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || int.TryParse(InternalValueAsString, FormatProvider, out int displayValue) == false || intValue != displayValue)
				InternalValueAsString = intValue.ToString(FormatProvider);
		}
		else if (value is uint uintValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || uint.TryParse(InternalValueAsString, FormatProvider, out uint displayValue) == false || uintValue != displayValue)
				InternalValueAsString = uintValue.ToString(FormatProvider);
		}
		else if (value is long longValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || long.TryParse(InternalValueAsString, FormatProvider, out long displayValue) == false || longValue != displayValue)
				InternalValueAsString = longValue.ToString(FormatProvider);
		}
		else if (value is ulong ulongValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || ulong.TryParse(InternalValueAsString, FormatProvider, out ulong displayValue) == false || ulongValue != displayValue)
				InternalValueAsString = ulongValue.ToString(FormatProvider);
		}
		else if (value is float floatValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || float.TryParse(InternalValueAsString, FormatProvider, out float displayValue) == false || floatValue != displayValue)
				InternalValueAsString = floatValue.ToString(FormatProvider);
		}
		else if (value is double doubleValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || double.TryParse(InternalValueAsString, FormatProvider, out double displayValue) == false || doubleValue != displayValue)
				InternalValueAsString = doubleValue.ToString(FormatProvider);
		}
		else if (value is decimal decimalValue)
		{
			if (string.IsNullOrWhiteSpace(InternalValueAsString) || decimal.TryParse(InternalValueAsString, FormatProvider, out decimal displayValue) == false || decimalValue != displayValue)
				InternalValueAsString = decimalValue.ToString(FormatProvider);
		}

		return InternalValueAsString;
	}

	private void OnDigitClicked(int digit)
	{
		if (AdditionalAttributes.IsDisabled())
			return;

		InternalValueAsString += digit.ToString(CultureInfo.InvariantCulture);

		if (InternalValueAsString.Length > 1 && InternalValueAsString[0] == '0' && InternalValueAsString.StartsWith($"0{DecimalSeparator}", StringComparison.Ordinal) == false)
			InternalValueAsString = InternalValueAsString.TrimStart('0');

		CurrentValueAsString = InternalValueAsString;
	}

	private void OnDecimalClicked()
	{
		if (AdditionalAttributes.IsDisabled())
			return;

		if (InternalValueAsString.Contains(DecimalSeparator, StringComparison.Ordinal))
			return;

		if (InternalValueAsString.Length == 0)
			InternalValueAsString = $"0{DecimalSeparator}";
		else
			InternalValueAsString += DecimalSeparator;
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

	private async Task OnInputKeyDown(KeyboardEventArgs args)
	{
		if (AdditionalAttributes.IsDisabled())
			return;

		if (UseArrowKeyNavigation == false)
		{
			OnKeyDown(args.Key);
			return;
		}

		if (args.Key == "ArrowDown")
			await JsRuntime.FocusElement(GetButtonId(0, LastColumnIndex));
		else if (args.Key == "ArrowUp")
			await JsRuntime.FocusElement(OnCustomButtonClicked != null ? GetButtonId(KeypadRows, 0) : GetButtonId(KeypadRows - 1, LastColumnIndex));
		else if (args.Key != "ArrowLeft" && args.Key != "ArrowRight")
			OnKeyDown(args.Key);
	}

	private async Task OnButtonKeyDown(KeyboardEventArgs args, int row, int column)
	{
		if (AdditionalAttributes.IsDisabled())
			return;

		if (row < KeypadRows)
			LastColumnIndex = column;

		if (UseArrowKeyNavigation == false)
		{
			OnKeyDown(args.Key);
			return;
		}

		if (args.Key == "ArrowUp")
		{
			if (row == KeypadRows)
				await JsRuntime.FocusElement(GetButtonId(KeypadRows - 1, LastColumnIndex));
			else if (row > 0)
				await JsRuntime.FocusElement(GetButtonId(row - 1, column));
			else if (DisplayInput)
				await JsRuntime.FocusElement(InputId);
			else if (OnCustomButtonClicked != null)
				await JsRuntime.FocusElement(GetButtonId(KeypadRows, 0));
			else
				await JsRuntime.FocusElement(GetButtonId(KeypadRows - 1, column));
		}
		else if (args.Key == "ArrowDown")
		{
			if (row == KeypadRows)
				await JsRuntime.FocusElement(DisplayInput ? InputId : GetButtonId(0, LastColumnIndex));
			else if (row < KeypadRows - 1)
				await JsRuntime.FocusElement(GetButtonId(row + 1, column));
			else if (OnCustomButtonClicked != null)
				await JsRuntime.FocusElement(GetButtonId(KeypadRows, 0));
			else if (DisplayInput)
				await JsRuntime.FocusElement(InputId);
			else
				await JsRuntime.FocusElement(GetButtonId(0, column));
		}
		else if (args.Key == "ArrowLeft")
		{
			if (row < KeypadRows)
				await JsRuntime.FocusElement(GetButtonId(row, column == 0 ? KeypadColumns - 1 : column - 1));
		}
		else if (args.Key == "ArrowRight")
		{
			if (row < KeypadRows)
				await JsRuntime.FocusElement(GetButtonId(row, column == KeypadColumns - 1 ? 0 : column + 1));
		}
		else
		{
			OnKeyDown(args.Key);
		}
	}

	private void OnKeyDown(string key)
	{
		if (NumberKeys.TryGetValue(key, out int value))
			OnDigitClicked(value);
		else if (key == "Backspace")
			OnBackspaceClicked();
		else if (SupportsDecimals && key == DecimalSeparator)
			OnDecimalClicked();
	}

    private void ResetStatus()
    {
        DisplayStatus &= ~InputStatus.BackgroundDanger;
        DisplayStatus &= ~InputStatus.BackgroundWarning;
        DisplayStatus &= ~InputStatus.BackgroundSuccess;
    }
}
