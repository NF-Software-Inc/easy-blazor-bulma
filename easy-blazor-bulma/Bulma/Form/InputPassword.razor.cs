using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// An input component for editing password values. Supported types are <see cref="string"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// There are 2 additional attributes that can be used: icon-class and message-class. Each of which apply CSS classes to the resulting elements as per their names.
/// </remarks>
public partial class InputPassword<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue>
{
	/// <summary>
	/// An icon to display within the input.
	/// </summary>
	[Parameter]
	public string? Icon { get; set; } = "password";

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

	private readonly string[] Filter = new string[] { "class", "icon-class", "message-class" };

	private readonly Type UnderlyingType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
	private bool IsNullable;

	private string? Message = null;
	private BulmaColors MessageColor = BulmaColors.Default;

    private bool IsCapsOn = false;

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

            return string.Join(' ', css, AdditionalAttributes.GetClass("icon-class"));
        }
    }

	private string MessageCssClass
    {
        get
        {
            var css = "help";

			if (MessageColor != BulmaColors.Default)
                css += ' ' + BulmaColorHelper.GetColorCss(MessageColor);

            return string.Join(' ', css, AdditionalAttributes.GetClass("message-class"));
        }
    }

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		IsNullable = Nullable.GetUnderlyingType(typeof(TValue)) != null;

		if (UnderlyingType != typeof(string))
			throw new InvalidOperationException($"Unsupported type param '{UnderlyingType.Name}'. Must be of type string.");
	}

	/// <inheritdoc />
	protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
        // Validate
        if (UseAutomaticStatusColors)
            ResetStatus();

        // Try parse
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

            validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "The {0} field must be a string.", DisplayName ?? FieldIdentifier.FieldName);
            return false;
        }
    }

    /// <inheritdoc />
    protected override string FormatValueAsString(TValue? value) => value switch
	{
		string stringValue => stringValue,
		_ => string.Empty
	};

	private void ResetStatus()
	{
		DisplayStatus &= ~InputStatus.BackgroundDanger;
		DisplayStatus &= ~InputStatus.BackgroundWarning;
		DisplayStatus &= ~InputStatus.BackgroundSuccess;
	}

	private void CheckCapsLock(KeyboardEventArgs args)
	{
		IsCapsOn = args.Key.Length == 1 && char.IsLetter(args.Key.First()) && char.IsLower(args.Key.First()) == false && args.ShiftKey == false;

		if (IsCapsOn)
		{
			Message = "Caps lock is on";
			MessageColor = BulmaColors.Yellow;
		}
		else if (MessageColor == BulmaColors.Yellow)
		{
			Message = null;
			MessageColor = BulmaColors.Default;
		}

		StateHasChanged();
	}
}
