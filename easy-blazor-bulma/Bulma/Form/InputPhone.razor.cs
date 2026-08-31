using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace easy_blazor_bulma;

/// <summary>
/// An input component for editing phone number values. Supported types are <see cref="string"/>.
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
public partial class InputPhone<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue>
{
	/// <summary>
	/// An icon to display within the input.
	/// </summary>
	[Parameter]
	public string? Icon { get; set; } = "phone";

	/// <summary>
	/// A mask to apply to the digits within the value.
	/// Each # character will be replaced with a single digit, all other characters are used as provided.
	/// Multiple masks can be provided, separated by a pipe '|' character.
	/// </summary>
	/// <remarks>
	/// Values that do not contain the same number of digits as there are # characters in the mask will not be formatted. For example, (###) ###-#### will display 5551234567 as (555) 123-4567.
	/// </remarks>
	[Parameter]
	public string? DisplayFormat { get; set; }

	/// <summary>
	/// An optional function to apply custom formatting to the value. Takes precedence over <see cref="DisplayFormat"/>.
	/// </summary>
	[Parameter]
	public Func<string?, string?>? Formatter { get; set; }

	/// <summary>
	/// A regular expression to validate the value with. Validation is applied after formatting.
	/// </summary>
	[Parameter]
	public string? ValidationExpression { get; set; }

	/// <summary>
	/// The message to display when the value does not match <see cref="ValidationExpression"/>.
	/// </summary>
	[Parameter]
	public string? ValidationErrorMessage { get; set; }

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
	private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);

	private Regex? Validator;
	private string? ValidatorExpression;

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
		if (UnderlyingType != typeof(string))
			throw new InvalidOperationException($"Unsupported type param '{UnderlyingType.Name}'. Must be of type string.");
	}

	/// <inheritdoc />
	protected override void OnParametersSet()
	{
		if (ValidatorExpression == ValidationExpression)
			return;

		ValidatorExpression = ValidationExpression;
		Validator = string.IsNullOrWhiteSpace(ValidationExpression) ? null : new Regex(ValidationExpression, RegexOptions.None, RegexTimeout);
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

		var formatted = FormatPhoneNumber(value);

		if (IsValid(formatted) == false)
		{
			result = default;

			if (UseAutomaticStatusColors)
				DisplayStatus |= InputStatus.BackgroundDanger;

			validationErrorMessage = ValidationErrorMessage ?? string.Format(CultureInfo.InvariantCulture, "The {0} field must be a valid phone number.", DisplayName ?? FieldIdentifier.FieldName);
			return false;
		}

		result = (TValue)(object)formatted;

		if (UseAutomaticStatusColors)
			DisplayStatus |= InputStatus.BackgroundSuccess;

		validationErrorMessage = null;
		return true;
	}

	/// <inheritdoc />
	protected override string FormatValueAsString(TValue? value) => value switch
	{
		string stringValue => FormatPhoneNumber(stringValue),
		_ => FormatPhoneNumber(null)
	};

	private string FormatPhoneNumber(string? value)
	{
		if (Formatter != null)
			return Formatter(value) ?? string.Empty;
		else if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(DisplayFormat))
			return value ?? string.Empty;

		var digits = value.Where(char.IsDigit).ToArray();
		var pattern = DisplayFormat
			.Split('|')
			.FirstOrDefault(x => x.Count(c => c == '#') == digits.Length);

		if (pattern == null)
			return value;

		var formatted = new StringBuilder(pattern.Length);
		var index = 0;

		foreach (var character in pattern)
			if (character == '#')
				formatted.Append(digits[index++]);
			else
				formatted.Append(character);

		return formatted.ToString();
	}

	private bool IsValid(string value)
	{
		if (Validator == null)
			return true;

		try
		{
			return Validator.IsMatch(value);
		}
		catch (RegexMatchTimeoutException)
		{
			return false;
		}
	}

	private void ResetStatus()
	{
		DisplayStatus &= ~InputStatus.BackgroundDanger;
		DisplayStatus &= ~InputStatus.BackgroundWarning;
		DisplayStatus &= ~InputStatus.BackgroundSuccess;
	}

	private void OnChange(ChangeEventArgs args)
	{
		CurrentValueAsString = args.Value?.ToString();
	}
}
